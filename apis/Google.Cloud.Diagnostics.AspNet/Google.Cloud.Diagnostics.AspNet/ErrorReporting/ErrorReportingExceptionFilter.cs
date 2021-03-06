﻿// Copyright 2017 Google Inc. All Rights Reserved.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Google.Api.Gax;
using Google.Cloud.Diagnostics.Common;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.ErrorReporting.V1Beta1;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Google.Protobuf.WellKnownTypes;
using System;

namespace Google.Cloud.Diagnostics.AspNet
{
    /// <summary>
    ///  Google Cloud Error Reporting <see cref="IExceptionFilter"/>.
    /// </summary>
    /// 
    /// <example>
    /// <code>
    /// public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    /// {
    ///   // Add a catch all for the uncaught exceptions.
    ///   string projectId = "[Google Cloud Platform project ID]";
    ///   string serviceName = "[Name of service]";
    ///   string version = "[Version of service]";
    ///   // Add a catch all for the uncaught exceptions.
    ///   filters.Add(ErrorReportingExceptionFilter.Create(projectId, serviceName, version));
    /// }
    /// </code>
    /// </example>
    /// 
    /// <remarks>
    /// Reports unhandled exceptions to Google Cloud Error Reporting.
    /// The filter should be registered first in ASP.NET MVC versions 1 and 2 and last
    /// in ASP.NET MVC versions 3 and higher. See:
    /// https://msdn.microsoft.com/en-us/library/system.web.mvc.iexceptionfilter.onexception(v=vs.118).aspx
    /// Docs: https://cloud.google.com/error-reporting/docs/
    /// </remarks>
    public class ErrorReportingExceptionFilter : IExceptionFilter
    {
        // The service context in which this error has occurred.
        // See: https://cloud.google.com/error-reporting/reference/rest/v1beta1/projects.events#ServiceContext
        private readonly ServiceContext _serviceContext;

        private readonly IConsumer<ReportedErrorEvent> _consumer;

        /// <summary>
        /// Creates an instance of <see cref="ErrorReportingExceptionFilter"/> using credentials as
        /// defined by <see cref="GoogleCredential.GetApplicationDefaultAsync"/>.
        /// </summary>
        /// <param name="projectId">The Google Cloud Platform project ID. Cannot be null.</param>
        /// <param name="serviceName">An identifier of the service, such as the name of the executable or job.
        ///     Cannot be null.</param>
        /// <param name="version">Represents the source code version that the developer provided. 
        ///     Cannot be null.</param>
        /// <param name="options">Optional, error reporting options.</param>
        public static ErrorReportingExceptionFilter Create(string projectId, string serviceName, string version,
            ErrorReportingOptions options = null)
        {
            GaxPreconditions.CheckNotNullOrEmpty(projectId, nameof(projectId));
            GaxPreconditions.CheckNotNullOrEmpty(serviceName, nameof(serviceName));
            GaxPreconditions.CheckNotNullOrEmpty(version, nameof(version));

            options = options ?? ErrorReportingOptions.Create(projectId);
            var consumer = options.CreateConsumer(projectId);
            return new ErrorReportingExceptionFilter(consumer, serviceName, version);
        }

        internal ErrorReportingExceptionFilter(
            IConsumer<ReportedErrorEvent> consumer, string serviceName, string version)
        {
            _consumer = GaxPreconditions.CheckNotNull(consumer, nameof(consumer));
            _serviceContext = new ServiceContext
            {
                Service = GaxPreconditions.CheckNotNull(serviceName, nameof(serviceName)),
                Version = GaxPreconditions.CheckNotNull(version, nameof(version)),
            };
        }

        /// <inheritdoc />
        public void OnException(ExceptionContext context)
        {
            var errorEvent = CreateReportRequest(context);
            _consumer.Receive(new[] { errorEvent });
        }

        /// <summary>
        /// Gets information about the HTTP request and response when the exception occured 
        /// and populates a <see cref="HttpRequestContext"/> object.
        /// </summary>
        private HttpRequestContext CreateHttpRequestContext(ExceptionContext context)
        {
            HttpRequestBase requestMessage = context?.HttpContext?.Request;
            HttpResponseBase responseMessage = context?.HttpContext?.Response;

            return new HttpRequestContext
            {
                Method = requestMessage?.HttpMethod ?? "",
                Url = requestMessage?.Url?.ToString() ?? "",
                UserAgent = requestMessage?.UserAgent ?? "",
                ResponseStatusCode = responseMessage?.StatusCode ?? 0,
            };
        }

        /// <summary>
        /// Gets infromation about the exception that occured and populates
        /// a <see cref="ReportedErrorEvent"/> object.
        /// </summary>
        private ReportedErrorEvent CreateReportRequest(ExceptionContext context)
        {
            ErrorContext errorContext = new ErrorContext
            {
                HttpRequest = CreateHttpRequestContext(context),
                ReportLocation = ErrorReportingUtils.CreateSourceLocation(context.Exception)
            };

            return new ReportedErrorEvent
            {
                Message = context.Exception.ToString() ?? "",
                Context = errorContext,
                ServiceContext = _serviceContext,
                EventTime = Timestamp.FromDateTime(DateTime.UtcNow),
            };
        }
    }
}
