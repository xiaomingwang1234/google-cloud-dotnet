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
using Xunit;

namespace Google.Storage.V1.Tests
{
    public class StorageUriTest
    {
        private readonly static string s_bucket1a = "bucket1";
        private readonly static string s_bucket1b = new string(s_bucket1a.ToCharArray());
        private readonly static string s_bucket2 = "bucket2";

        private readonly static string s_object1a = "object1";
        private readonly static string s_object1b = new string(s_object1a.ToCharArray());
        private readonly static string s_object2 = "object2";

        [Fact]
        public void CheckTestStrings()
        {
            // Just a sanity check...
            Assert.Equal(s_bucket1a, s_bucket1b);
            Assert.NotSame(s_bucket1a, s_bucket1b);
            Assert.NotEqual(s_bucket1a, s_bucket2);

            Assert.Equal(s_object1a, s_object1b);
            Assert.NotSame(s_object1a, s_object1b);
            Assert.NotEqual(s_object1a, s_object2);
        }

        [Fact]
        public void NullBucketNameRejected()
        {
            Assert.Throws<ArgumentNullException>(() => new StorageUri(null, "object"));
        }

        [Theory]
        [InlineData("")]
        [InlineData("x")]
        [InlineData("xy")]
        [InlineData("bucketA")]
        [InlineData("bucket!")]
        [InlineData("bucket+")]
        [InlineData("bucket ")]
        public void InvalidBucketNamesRejected(string bucket)
        {
            Assert.Throws<ArgumentException>(() => new StorageUri(bucket, "object"));
        }

        [Fact]
        public void EmptyObjectNameBecomesNull()
        {
            var uri = new StorageUri("bucket", "");
            Assert.Equal("bucket", uri.Bucket);
            Assert.Null(uri.Object);
        }

        [Fact]
        public void Equality()
        {
            AssertEquality(new StorageUri(s_bucket1a, null), new StorageUri(s_bucket1a, null));
            AssertEquality(new StorageUri(s_bucket1a, null), new StorageUri(s_bucket1b, null));
            AssertEquality(new StorageUri(s_bucket1a, null), new StorageUri(s_bucket1b, ""));
            AssertInequality(new StorageUri(s_bucket1a, null), new StorageUri(s_bucket2, null));

            AssertEquality(new StorageUri(s_bucket1a, s_object1a), new StorageUri(s_bucket1b, s_object1a));
            AssertEquality(new StorageUri(s_bucket1a, s_object1a), new StorageUri(s_bucket1b, s_object1b));
            AssertInequality(new StorageUri(s_bucket1a, s_object1a), new StorageUri(s_bucket2, s_object1a));
            AssertInequality(new StorageUri(s_bucket1a, s_object1a), new StorageUri(s_bucket1a, s_object2));

            AssertInequality(new StorageUri(s_bucket1a, s_object1a), null);
        }

        [Fact]
        public void EqualityOperatorsWithNull()
        {
            StorageUri nullUri = null;
            StorageUri nonNullUri = new StorageUri("bucket", "object");
            Assert.True(nullUri == null);
            Assert.False(nullUri != null);
            Assert.False(nonNullUri == null);
            Assert.True(nonNullUri != null);
            Assert.False(null == nonNullUri);
            Assert.True(null != nonNullUri);
        }

        [Theory]
        [InlineData("gs://bucket", "bucket", null)]
        [InlineData("gs://bucket/object", "bucket", "object")]
        [InlineData("gs://bucket/first/second", "bucket", "first/second")]
        // FIXME: Escaping
        // [InlineData("gs://bucket/a+b", "bucket", "a b")]
        public void ParseAndReformat(string uri, string expectedBucket, string expectedObject)
        {
            var parsed = StorageUri.Parse(uri);
            Assert.Equal(expectedBucket, parsed.Bucket);
            Assert.Equal(expectedObject, parsed.Object);
            Assert.Equal(uri, parsed.ToString());
        }

        private static void AssertEquality(StorageUri left, StorageUri right)
        {
            Assert.True(left.Equals(right));
            Assert.True(left.Equals((object)right));
            Assert.True(left == right);
            Assert.False(left != right);
            Assert.Equal(left.GetHashCode(), right.GetHashCode());
        }

        private static void AssertInequality(StorageUri left, StorageUri right)
        {
            Assert.False(left.Equals(right));
            Assert.False(left.Equals((object) right));
            Assert.False(left == right);
            Assert.True(left != right);
            // Not actually guaranteed, but we can make sure we provide the right values for tests.
            if (!ReferenceEquals(right, null))
            {
                Assert.NotEqual(left.GetHashCode(), right.GetHashCode());
            }
        }
    }
}
