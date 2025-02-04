﻿using NetTopologySuite.Densify;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.Tests.NUnit.Densify
{
    public class DensifierTest : GeometryTestCase
    {
        private const double TOLERANCE = 1e-6;

        [Test]
        public void TestLine()
        {
            CheckDensify("LINESTRING (0 0, 30 40, 35 35)",
                10,
                "LINESTRING (0 0, 6 8, 12 16, 18 24, 24 32, 30 40, 35 35)");
        }

        [Test]
        public void TestLineOfToleranceLength()
        {
            CheckDensify("LINESTRING (0 0, 10 0)",
                10, "LINESTRING (0 0, 10 0)");
        }

        [Test]
        public void TestLineWithToleranceLengthSeg()
        {
            CheckDensify("LINESTRING (0 0, 12 0, 22 0, 34 0)",
                10, "LINESTRING (0 0, 6 0, 12 0, 22 0, 28 0, 34 0)");
        }

        [Test]
        public void TestLineEmpty()
        {
            CheckDensify("LINESTRING EMPTY",
                10, "LINESTRING EMPTY");
        }

        [Test]
        public void TestPointUnchanged()
        {
            CheckDensify("POINT (0 0)",
                10, "POINT (0 0)");
        }

        [Test]
        public void TestPolygonEmpty()
        {
            CheckDensify("POLYGON EMPTY",
                10, "POLYGON EMPTY");
        }

        [Test]
        public void TestBox()
        {
            CheckDensify("POLYGON ((10 30, 30 30, 30 10, 10 10, 10 30))",
                10,
                "POLYGON ((10 10, 10 20, 10 30, 20 30, 30 30, 30 20, 30 10, 20 10, 10 10))");
        }

        [Test]
        public void TestBoxNoValidate()
        {
            CheckDensifyNoValidate("POLYGON ((10 30, 30 30, 30 10, 10 10, 10 30))",
                10,
                "POLYGON ((10 10, 10 20, 10 30, 20 30, 30 30, 30 20, 30 10, 20 10, 10 10))");
        }

        [Test]
        public void TestDimension2d()
        {
            var gf = NtsGeometryServices.Instance.CreateGeometryFactory();
            var line = gf.CreateLineString(new [] { new Coordinate(1, 2), new Coordinate(3, 4) });
            Assert.That(line.CoordinateSequence.Dimension, Is.EqualTo(2));

            line = (LineString)Densifier.Densify(line, 0.1);
            Assert.That(line.CoordinateSequence.Dimension, Is.EqualTo(2));
        }

        [Test]
        public void TestDimension3d()
        {
            var gf = NtsGeometryServices.Instance.CreateGeometryFactory(); ;
            var line = gf.CreateLineString(new[] { new CoordinateZ(1, 2, 3), new CoordinateZ(3, 4, 5) });
            Assert.That(line.CoordinateSequence.Dimension, Is.EqualTo(3));

            line = (LineString)Densifier.Densify(line, 0.1);
            Assert.That(line.CoordinateSequence.Dimension, Is.EqualTo(3));
        }

        private void CheckDensify(string wkt, double distanceTolerance, string wktExpected)
        {
            var geom = Read(wkt);
            var expected = Read(wktExpected);
            var actual = Densifier.Densify(geom, distanceTolerance);
            CheckEqual(expected, actual, TOLERANCE);
        }

        /*
         * Note: it's hard to construct a geometry which would actually be invalid when densified.
         * This test just checks that the code path executes.
         */
        private void CheckDensifyNoValidate(string wkt, double distanceTolerance, string wktExpected)
        {
            var geom = Read(wkt);
            var expected = Read(wktExpected);
            var den = new Densifier(geom);
            den.DistanceTolerance = distanceTolerance;
            den.Validate = false;
            var actual = den.GetResultGeometry();
            CheckEqual(expected, actual, TOLERANCE);
        }

    }
}
