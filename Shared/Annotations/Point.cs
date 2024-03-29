﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocsWASM.Shared.Annotations
{
	public struct Point
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Point(double x, double y)
		{
			X = x;
			Y = y;
		}
	}

	public struct Size
	{
		public int Width { get; set; }
		public int Height { get; set; }

		public Size(int width, int height)
		{
			Width = width;
			Height = height;
		}
	}

}
