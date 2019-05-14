﻿using System;
using System.Runtime.CompilerServices;

namespace MESMARCIN
{
    class Program
    {
        static void Main(string[] args)
        {
            var grid = new Grid();
            var newGrid = CalculateLocalMatrix(grid);
            var hL = newGrid.Elements[0].HL;
            MatrixHelper.PrintMatrix(hL);
            var ue = new UniversalElement();
            Console.ReadKey();
        }

        public static Grid CalculateLocalMatrix(Grid grid)
        {
            const int K = 30;
            const double c = 2000;
            const double ro = 1000;
            const double alfa = 0.3;
            var universalElemenet = new UniversalElement();
            for (var i = 0; i < grid.Elements.Length; i++)
            {
                var x1 = grid.Nodes[grid.Elements[i].Id[0]].X;
                var y1 = grid.Nodes[grid.Elements[i].Id[0]].Y;
                var x2 = grid.Nodes[grid.Elements[i].Id[1]].X;
                var y2 = grid.Nodes[grid.Elements[i].Id[1]].Y;
                var x3 = grid.Nodes[grid.Elements[i].Id[2]].X;
                var y3 = grid.Nodes[grid.Elements[i].Id[2]].Y;
                var x4 = grid.Nodes[grid.Elements[i].Id[3]].X;
                var y4 = grid.Nodes[grid.Elements[i].Id[3]].Y;
                var elementH = new double[,]
                {
                    {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}
                };
                var elementC = new double[,]
                {
                    {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}
                };
                for (var j = 0; j < GlobalData.nPc * GlobalData.nPc; j++)
                {
                    var jakobian = new Jakobian(x1, y1, x2, y2, x3, y3, x4, y4, j, universalElemenet);
                    var dNdX = new double[4];
                    var dNdY = new double[4];
                    var N = new double[4];

                    //macierz H
                    for (var k = 0; k < 4; k++)
                    {
                        dNdX[k] = jakobian.ValueT[0, 0] * universalElemenet.dNdE[j, k] +
                                    jakobian.ValueT[0, 1] * universalElemenet.dNdN[j, k];
                    }
                    for (var k = 0; k < 4; k++)
                    {
                        dNdY[k] = jakobian.ValueT[1, 0] * universalElemenet.dNdE[j, k] +
                                  jakobian.ValueT[1, 1] * universalElemenet.dNdN[j, k];
                    }


                    var matrixDx = MatrixHelper.TranspositionAndMultipication(dNdX);
                    var matrixDy = MatrixHelper.TranspositionAndMultipication(dNdY);
                    var detAndWeights = jakobian.Det * universalElemenet.weightsC[0] * universalElemenet.weightsC[1];
                    matrixDx = MatrixHelper.MatrixScalarMultiplication(matrixDx, detAndWeights);
                    matrixDy = MatrixHelper.MatrixScalarMultiplication(matrixDy, detAndWeights);

                    var sumMatrix = MatrixHelper.AddMatrix(matrixDx, matrixDy);
                    sumMatrix = MatrixHelper.MatrixScalarMultiplication(sumMatrix, K);
                    elementH = MatrixHelper.AddMatrix(elementH, sumMatrix);
                    

                    //MACIERZ C
                    for (var k = 0; k < 4; k++)
                    {
                        N[k] = universalElemenet.N[j, k];
                    }

                    var cRazyCT = MatrixHelper.TranspositionAndMultipication(N);
                    var actualC = MatrixHelper.MatrixScalarMultiplication(cRazyCT, detAndWeights * ro * c);
                    elementC = MatrixHelper.AddMatrix(elementC, actualC);
                }
                //Macierz H + czescioweP 
                for (var j = 0; j < 8; j++)
                {
                    var elementToAddToH = new double[,]
                    {
                        {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}, {0, 0, 0, 0}
                    };
                    //TODO!!! Jak wyzej np dla N
                }

                grid.Elements[i].HL = elementH;
                grid.Elements[i].CL = elementC;
            }

           

            return grid;
        }
    }
}
