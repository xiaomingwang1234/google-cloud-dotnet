// Copyright 2016 Google Inc. All Rights Reserved.
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

using System;
using System.Text.RegularExpressions;

namespace Google.Storage.V1
{
    /// <summary>
    /// Storage URI consisting of a bucket name and an optional object name.
    /// This class is immutable.
    /// </summary>
    public sealed class StorageUri : IEquatable<StorageUri>
    {
        // TODO: Move all name validation here?

        private static readonly Regex ValidUri = new Regex(@"^gs://(?<bucket>[0-9a-z.\-_]{1,222})(?:/(?<object>.*))?$");

        /// <summary>
        /// The name of the bucket. Never null.
        /// </summary>
        public string Bucket { get; }
        
        /// <summary>
        /// The name of the object within the bucket. May be null,
        /// indicating that the storage URI represents the bucket itself.
        /// </summary>
        public string Object { get; }

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        /// <remarks>
        /// If <paramref name="objectName"/> is empty, it is converted to null as far
        /// as the <see cref="Object"/> property is concerned.
        /// </remarks>
        /// <param name="bucketName">The name of the bucket. Must not be null.</param>
        /// <param name="objectName">The name of the object within the bucket. May be null or empty.</param>
        public StorageUri(string bucket, string objectName)
        {
            StorageClientImpl.ValidateBucketName(bucket);
            if (objectName == "")
            {
                objectName = null;
            }
            if (objectName != null)
            {
                StorageClientImpl.ValidateObjectName(objectName, nameof(objectName));
            }
            Bucket = bucket;
            Object = objectName;
        }

        /// <inheritdoc />
        public bool Equals(StorageUri other) =>
            other != null &&
            other.Bucket == Bucket &&
            other.Object == Object;

        /// <summary>
        /// Compares the given Storage URIs for equality.
        /// </summary>
        /// <param name="left">The first Storage URI to compare.</param>
        /// <param name="right">The second Storage URI to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> and <paramref name="right"/> are equal, <c>false</c> otherwise.</returns>
        public static bool operator==(StorageUri left, StorageUri right) => 
            ReferenceEquals(left, right) || (!ReferenceEquals(left, null) && left.Equals(right));

        /// <summary>
        /// Compares the given Storage URIs for equality.
        /// </summary>
        /// <param name="left">The first Storage URI to compare.</param>
        /// <param name="right">The second Storage URI to compare.</param>
        /// <returns><c>false</c> if <paramref name="left"/> and <paramref name="right"/> are equal, <c>true</c> otherwise.</returns>
        public static bool operator!=(StorageUri left, StorageUri right) => !(left == right);

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as StorageUri);


        /// <inheritdoc />
        public override int GetHashCode()
        {
            int hash = 23;
            hash = hash * 31 + Bucket.GetHashCode();
            hash = hash * 31 + (Object?.GetHashCode() ?? 0);
            return hash;
        }

        // FIXME: Escaping

        /// <inheritdoc />
        public override string ToString() => Object == null ? $"gs://{Bucket}" : $"gs://{Bucket}/{Object}";


        /// <summary>
        /// Parses the given storage URI.
        /// </summary>
        /// <param name="uri">The URI to parse. Must not be null.</param>
        /// <returns>A <see cref="StorageUri"/> representing the parsed data from <paramref name="uri"/>.</returns>
        public static StorageUri Parse(string uri)
        {
            var match = ValidUri.Match(uri);
            if (!match.Success)
            {
                throw new FormatException($"Invalid Storage uri: '{uri}'");
            }
            var bucket = match.Groups["bucket"].Value;
            var objectName = match.Groups["object"].Value;
            // FIXME: Escaping
            return new StorageUri(bucket, objectName);
        }
    }
}
