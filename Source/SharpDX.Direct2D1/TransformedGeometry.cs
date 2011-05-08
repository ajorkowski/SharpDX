﻿// Copyright (c) 2010-2011 SharpDX - Alexandre Mutel
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;

namespace SharpDX.Direct2D1
{
    public partial class TransformedGeometry
    {
        /// <summary>
        /// Default Constructor for a <see cref = "SharpDX.Direct2D1.TransformedGeometry" />.
        /// </summary>
        /// <param name="factory">an instance of <see cref = "SharpDX.Direct2D1.Factory" /></param>
        /// <param name="geometrySource"></param>
        /// <param name="matrix3X2"></param>
        public TransformedGeometry(Factory factory, Geometry geometrySource, Matrix3x2 matrix3X2) : base(IntPtr.Zero)
        {
            TransformedGeometry temp;
            factory.CreateTransformedGeometry(geometrySource, ref matrix3X2, out temp);
            NativePointer = temp.NativePointer;
        }
    }
}