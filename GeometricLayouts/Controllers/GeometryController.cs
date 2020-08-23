using System;
using System.Collections.Generic;
using GeometricLayouts.Models;
using Microsoft.AspNetCore.Mvc;

namespace GeometricLayouts.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GeometryController : ControllerBase
    {
        const int TotalRowPixels = 60;
        const int TotalColumnPixels = 60;
        const int NonHypotenuseSide = 10;

        [HttpGet]
        public object AvailableEndpoints()
        {
            return Ok(new
            {
                availableEndpoints = new
                {
                    coordinates = "/coordinates?row=ROW_DATA&column=COLUMN_DATA",
                    rowcolumn = "/rowcolumn?v1x=V1X_DATA&v1y=V1Y_DATA&v2x=V2X_DATA&v2y=V2Y_DATA&v3x=V3X_DATA&v3y=V3Y_DATA"
                }
            });
        }

        [HttpGet("Coordinates")]
        public IActionResult Coordinates(char row, int column)
        {
            int totalRows = TotalRowPixels / NonHypotenuseSide;
            int totalColumns = (TotalColumnPixels / NonHypotenuseSide) * 2;
            int rowNumber = GetRowNumber(row);
            var points = new List<Point>();

            if (rowNumber < 0 || rowNumber >= totalRows)
            {
                return BadRequest(new { message = "'row' should be in A-F range" });
            }

            if (column < 1 || column > totalColumns)
            {
                return BadRequest(new { message = $"'column' should be in 1-{totalColumns - 1} range" });
            }

            var fistPoint = new Point { X = rowNumber * NonHypotenuseSide, Y = ((int)Math.Ceiling(column / 2f) - 1) * NonHypotenuseSide };
            points.Add(fistPoint);
            points.Add(new Point { X = fistPoint.X + NonHypotenuseSide, Y = fistPoint.Y + NonHypotenuseSide });

            if (column % 2 == 0)
            {
                points.Add(new Point { X = fistPoint.X, Y = fistPoint.Y + NonHypotenuseSide });
            }
            else
            {
                points.Add(new Point { X = fistPoint.X + NonHypotenuseSide, Y = fistPoint.Y });
            }

            return Ok(new { points });
        }

        [HttpGet("RowColumn")]
        public IActionResult RowColumn(int v1x, int v1y, int v2x, int v2y, int v3x, int v3y)
        {
            if (v1x < 0 || v1x > TotalRowPixels || v1y < 0 || v1y > TotalColumnPixels)
            {
                return BadRequest(new { message = $"'v1x' should be in 0-{TotalRowPixels} range and 'v1y' should be in 0-{TotalColumnPixels} range " });
            }

            if (v2x < 0 || v2x > TotalRowPixels || v2y < 0 || v2y > TotalColumnPixels)
            {
                return BadRequest(new { message = $"'v2x' should be in 0-{TotalRowPixels} range and 'v2y' should be in 0-{TotalColumnPixels} range " });
            }

            if (v3x < 0 || v3x > TotalRowPixels || v3y < 0 || v3y > TotalColumnPixels)
            {
                return BadRequest(new { message = $"'v3x' should be in 0-{TotalRowPixels} range and 'v3y' should be in 0-{TotalColumnPixels} range " });
            }

            if (v1x % 10 != 0 || v1y % 10 != 0 || v2x % 10 != 0 || v2y % 10 != 0 || v3x % 10 != 0 || v3y % 10 != 0)
            {
                return BadRequest(new { message = "Vertex coordinates should be divisible by 10" });
            }

            int minRow = Math.Min(Math.Min(v1x, v2x), v3x);
            int minColumn = Math.Min(Math.Min(v1y, v2y), v3y);

            int rowNumber = minRow / NonHypotenuseSide;

            int repeatedY = v1y == v2y || v1y == v3y ? v1y : v2y;

            int rowColumn = (repeatedY / NonHypotenuseSide) * 2 + (repeatedY == minColumn ? 1 : 0);

            return Ok($"{(char)(rowNumber + 'A')}{rowColumn}");
        }

        private int GetRowNumber(char row)
        {
            return row - 'A';
        }
    }
}