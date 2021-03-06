﻿// The MIT License (MIT)

// Copyright (c) 2015 Ben Abelshausen

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using NUnit.Framework;
using Reminiscence.Arrays;
using Reminiscence.IO;
using System;
using System.IO;

namespace Reminiscence.Tests.Arrays
{
    /// <summary>
    /// Contains tests for array.
    /// </summary>
    public class ArrayTests
    {
        /// <summary>
        /// Tests argument checks.
        /// </summary>
        [Test]
        public void ArgumentTest()
        {
            using (var map = new MemoryMapStream())
            {
                using (var array = new Array<uint>(map, 1000))
                {
                    Assert.AreEqual(1000, array.Length);
                    Assert.Catch<ArgumentOutOfRangeException>(() =>
                    {
                        array[1001] = 10;
                    });
                    Assert.Catch<ArgumentOutOfRangeException>(() =>
                    {
                        array[-1] = 10;
                    });

                    uint value;
                    Assert.Catch<ArgumentOutOfRangeException>(() =>
                    {
                        value = array[1001];
                    });
                    Assert.Catch<ArgumentOutOfRangeException>(() =>
                    {
                        value = array[-1];
                    });
                }
            }
        }

        /// <summary>
        /// Tests for the array when it has zero-size.
        /// </summary>
        [Test]
        public void ZeroSizeTest()
        {
            using (var map = new MemoryMapStream())
            {
                using (var array = new Array<uint>(map, 0))
                {
                    Assert.AreEqual(0, array.Length);
                }
                using (var array = new Array<uint>(map, 100))
                {
                    array.Resize(0);
                    Assert.AreEqual(0, array.Length);
                }
            }
        }

        /// <summary>
        /// A test for the array comparing it to a regular array.
        /// </summary>
        [Test]
        public void CompareToArrayTest()
        {
            var randomGenerator = new System.Random(66707770); // make this deterministic 

            using (var map = new MemoryMapStream())
            {
                using (var array = new Array<uint>(map, 1000))
                {
                    var arrayExpected = new uint[1000];

                    for (uint i = 0; i < 1000; i++)
                    {
                        if (randomGenerator.Next(4) >= 2)
                        { // add data.
                            arrayExpected[i] = i;
                            array[i] = i;
                        }
                        else
                        {
                            arrayExpected[i] = int.MaxValue;
                            array[i] = int.MaxValue;
                        }
                        Assert.AreEqual(arrayExpected[i], array[i]);
                    }

                    for (var i = 0; i < 1000; i++)
                    {
                        Assert.AreEqual(arrayExpected[i], array[i]);
                    }
                }
            }
        }

        /// <summary>
        /// A test for the array comparing it to a regular array without any caching.
        /// </summary>
        [Test]
        public void CompareToArrayWithNoCacheTest()
        {
            var randomGenerator = new System.Random(66707770); // make this deterministic 

            using (var map = new MemoryMapStream())
            {
                using (var array = new Array<uint>(map, 1000, ArrayProfile.NoCache))
                {
                    var arrayExpected = new uint[1000];

                    for (uint i = 0; i < 1000; i++)
                    {
                        if (randomGenerator.Next(4) >= 2)
                        { // add data.
                            arrayExpected[i] = i;
                            array[i] = i;
                        }
                        else
                        {
                            arrayExpected[i] = int.MaxValue;
                            array[i] = int.MaxValue;
                        }
                        Assert.AreEqual(arrayExpected[i], array[i]);
                    }

                    for (var i = 0; i < 1000; i++)
                    {
                        Assert.AreEqual(arrayExpected[i], array[i]);
                    }
                }
            }
        }

        /// <summary>
        /// Tests resizing the array.
        /// </summary>
        [Test]
        public void ResizeTests()
        {
            var randomGenerator = new System.Random(66707770); // make this deterministic 

            using (var map = new MemoryMapStream())
            {
                using (var array = new Array<uint>(map, 1000, 256, 256, 32))
                {
                    var arrayExepected = new uint[1000];

                    for (uint i = 0; i < 1000; i++)
                    {
                        if (randomGenerator.Next(4) >= 2)
                        { // add data.
                            arrayExepected[i] = i;
                            array[i] = i;
                        }
                        else
                        {
                            arrayExepected[i] = int.MaxValue;
                            array[i] = int.MaxValue;
                        }

                        Assert.AreEqual(arrayExepected[i], array[i]);
                    }

                    Array.Resize<uint>(ref arrayExepected, 335);
                    array.Resize(335);

                    Assert.AreEqual(arrayExepected.Length, array.Length);
                    for (int i = 0; i < arrayExepected.Length; i++)
                    {
                        Assert.AreEqual(arrayExepected[i], array[i]);
                    }
                }

                using (var array = new Array<uint>(map, 1000, 256, 256, 32))
                {
                    var arrayExpected = new uint[1000];

                    for (uint i = 0; i < 1000; i++)
                    {
                        if (randomGenerator.Next(4) >= 1)
                        { // add data.
                            arrayExpected[i] = i;
                            array[i] = i;
                        }
                        else
                        {
                            arrayExpected[i] = int.MaxValue;
                            array[i] = int.MaxValue;
                        }

                        Assert.AreEqual(arrayExpected[i], array[i]);
                    }

                    Array.Resize<uint>(ref arrayExpected, 1235);
                    var oldSize = array.Length;
                    array.Resize(1235);

                    Assert.AreEqual(arrayExpected.Length, array.Length);
                    for (int i = 0; i < arrayExpected.Length; i++)
                    {
                        Assert.AreEqual(arrayExpected[i], array[i], 
                            string.Format("Array element not equal at index: {0}. Expected {1}, found {2}",
                                i, array[i], arrayExpected[i]));
                    }
                }
            }
        }

        /// <summary>
        /// Tests write to stream.
        /// </summary>
        [Test]
        public void TestWriteToAndReadFrom()
        {
            using (var map = new MemoryMapStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var array = new Array<int>(map, 10))
                    {
                        for (var i = 0; i < array.Length; i++)
                        {
                            array[i] = i + 100;
                        }

                        array.CopyTo(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (var array1 = new Array<int>(map, array.Length))
                        {
                            array1.CopyFrom(memoryStream);
                            for (var i = 0; i < array.Length; i++)
                            {
                                Assert.AreEqual(array[i], array1[i]);
                            }
                        }
                    }
                }
            }

            using (var map = new MemoryMapStream())
            {
                using (var memoryStream = new MemoryStream())
                {
                    using (var array = new Array<int>(map, 10000, 32, 32, 2))
                    {
                        for (var i = 0; i < array.Length; i++)
                        {
                            array[i] = i + 100;
                        }

                        array.CopyFrom(memoryStream);
                        memoryStream.Seek(0, SeekOrigin.Begin);

                        using (var array1 = new Array<int>(map, array.Length))
                        {
                            array.CopyFrom(memoryStream);
                            for (var i = 0; i < array.Length; i++)
                            {
                                Assert.AreEqual(array[i], array1[i]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests reading/writing an array that has a lenght smaller than the default buffer size.
        /// </summary>
        [Test]
        public void TestArraySmallerThanBuffer()
        {
            var data = new byte[16]; // room for 4 int's
            using(var map = new MemoryMapStream(new MemoryStream(data)))
            {
                // create a fixed-length array with one accessor.
                var array = new Array<int>(map.CreateInt32(4));

                Assert.AreEqual(4, array.Length);

                array[0] = 0;
                array[1] = 1;
                array[2] = 2;
                array[3] = 3;

                Assert.AreEqual(0, array[0]);
                Assert.AreEqual(1, array[1]);
                Assert.AreEqual(2, array[2]);
                Assert.AreEqual(3, array[3]);
            }
        }
    }
}