using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MatrixCalculatorWPF
{
    class Matrix
    {
        int rows;
        int columns;
        double[,] values;
        private double steps = 0;

        public bool WasAnalyzed { get; set; } = false;
        public double[,] Values { get => values; set => values = value; }
        public bool IsSquare { get; set; } = true;
        public int MatrixOrder { get; set; } = 0;
        public bool IsZero { get; set; } = true;
        public bool IsIdentity { get; set; } = true;
        public bool IsDiagonal { get; set; } = true;
        public bool IsTriangularUP { get; set; } = true;
        public bool IsTriangularLOW { get; set; } = true;
        public bool IsSymmetric { get; set; } = true;
        public bool IsSingular { get; set; } = true;
        public double TraceValue { get; set; } = 0;
        public double Determinant { get; set; } = 0;
        public int Rows { get => rows; set => rows = value; }
        public int Columns { get => columns; set => columns = value; }

        public Matrix(int rows, int columns)
        {
            this.rows = rows;
            this.columns = columns;

            values = new double[rows, columns];

        }

        public void FillMatrix(double[,] inputTab)
        {
            values = inputTab;
        }
        public void Analyze()
        {
            WasAnalyzed = true;

            if (rows != columns)
                IsSquare = false;

            if(IsSquare)
            {
                MatrixOrder = rows;
                CalcTrace();
                steps = 0;
                Determinant = CalcDeterminant(Values);
                if (Determinant != 0)
                    IsSingular = false;

                Debug.WriteLine("STEPS TO CALC ORDER: " + MatrixOrder + " MATRIX = " + steps);
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < columns; j++)
                    {                      
                        if (j == i && IsIdentity)                     // Check identity
                        {
                            if (values[i, j] != 1)
                            {
                                IsIdentity = false;
                            }
                        }
                        else if(IsIdentity || IsDiagonal)             // Check diagonal too
                        {
                            if (values[i, j] != 0)
                            {
                                IsIdentity = false;
                                IsDiagonal = false;
                            }
                        }

                        if (values[i, j] != 0 && IsZero)              // Check zero
                        {
                            IsZero = false;
                        }

                        if(i < j && IsTriangularLOW)                  // Check low triangularity      
                        {
                            if (values[i, j] != 0)
                                IsTriangularLOW = false;
                        }

                        if (i > j && IsTriangularUP)                  // Check upper triangularity
                        {
                            if (values[i, j] != 0)
                                IsTriangularUP = false;
                        }

                        if(values[i,j] != values[j,i] && i!=j)        // Check symmetric
                        {
                            IsSymmetric = false;
                        }
                    }
                }
            }
            else
            {
                IsIdentity = false;
                IsDiagonal = false;
                IsZero = false;
                IsTriangularUP = false;
                IsTriangularLOW = false;
                IsSymmetric = false;
            }
        }

        private void CalcTrace()
        {
            double tmp = 0;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (i == j)
                        tmp += values[i, j];
                }
            }

            TraceValue = tmp;
        }

        private double CalcDeterminant(double[,] matrix)
        {
            steps++;
            double tmpDeterminant = 0;
            int tmpOrder = matrix.GetLength(0);

            if (tmpOrder == 1)
            {
                return matrix[0, 0];
            }
            else if (tmpOrder == 2)
            {
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];
            }
            else if (tmpOrder == 3)
            {
                double diagonalPlus = matrix[0, 0] * matrix[1, 1] * matrix[2, 2];
                double firstTrianglePlus = matrix[1, 0] * matrix[2, 1] * matrix[0, 2];
                double secondTrianglePlus = matrix[2, 0] * matrix[0, 1] * matrix[1, 2];

                double diagonalMinus = matrix[0, 2] * matrix[1, 1] * matrix[2, 0];
                double firstTriangleMinus = matrix[1, 2] * matrix[2, 1] * matrix[0, 0];
                double secondTriangleMinus = matrix[2, 2] * matrix[1, 0] * matrix[0, 1];

                return diagonalPlus + firstTrianglePlus + secondTrianglePlus - diagonalMinus - firstTriangleMinus - secondTriangleMinus;
            }
            else if (tmpOrder > 3)
            {
                int explication = 1;
                int m, n;

                double[,] tmpMatrix = new double[tmpOrder - 1, tmpOrder - 1];

                for (int i = 0; i < tmpOrder; i++)
                {
                    m = 0;
                    for (int j = 0; j < tmpOrder; j++)
                    {
                        n = 0;
                        for (int k = 0; k < tmpOrder; k++)
                        {
                            if (j != explication - 1 && k != i)
                            {
                                tmpMatrix[m, n] = matrix[j, k];
                                n++;
                            }
                        }
                        if (j != explication - 1)
                            m++;
                    }
                    tmpDeterminant += matrix[explication - 1, i] * Math.Pow((-1), explication + i + 1) * CalcDeterminant(tmpMatrix);
                }
            }

            return tmpDeterminant;
        }

        public Matrix TransposeMatrix()
        {
            Matrix transposedMatrix = new Matrix(columns, rows);
            double[,] transposedValues = new double[columns, rows];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    transposedValues[j, i] = values[i, j];
                }
            }

            transposedMatrix.FillMatrix(transposedValues);

            return transposedMatrix;
        }

        public Matrix CofactorMatrix()
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[,] returnValues = new double[rows, columns];

            Matrix tmpMatrix = new Matrix(rows - 1, columns - 1);
            double[,] tmpValues = new double[rows - 1, columns - 1];


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    int x = 0;
                    for (int k = 0; k < rows; k++)
                    {
                        if(i != k)
                        {
                            int y = 0;
                            for (int l = 0; l < columns; l++)
                            {
                                if (j != l)
                                {
                                    tmpValues[x, y] = Values[k,l];
                                    y++;
                                }
                            }
                            x++;
                        }
                    }
                    tmpMatrix.FillMatrix(tmpValues);
                    returnValues[i, j] = Math.Pow(-1, i + j) * tmpMatrix.CalcDeterminant(tmpValues);
                }
            }

            returnMatrix.FillMatrix(returnValues);
            return returnMatrix;
        }

        public Matrix InverseMatrix()
        {
            Matrix returnMatrix = CofactorMatrix().TransposeMatrix();
            returnMatrix = returnMatrix.MultiplyByNumber(1/Determinant);
            return returnMatrix;
        }

        public Matrix MultiplyByNumber(double scalar)
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[,] multipliedValues = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    multipliedValues[i, j] = scalar * Values[i, j];
                }
            }

            returnMatrix.FillMatrix(multipliedValues);

            return returnMatrix;

        }

        public Matrix ReplaceTwoRows(int first, int second)
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[] tmpRow = new double[columns];
            double[,] tmpValues = Values;

            for (int i = 0; i < columns; i++)
            {
                tmpRow[i] = tmpValues[first, i];
            }

            for (int i = 0; i < columns; i++)
            {
                tmpValues[first, i] = tmpValues[second, i];
            }
            for (int i = 0; i < columns; i++)
            {
                tmpValues[second, i] = tmpRow[i];
            }
            returnMatrix.FillMatrix(tmpValues);

            return returnMatrix;
        }

        public Matrix ReplaceTwoColumns(int first, int second)
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[] tmpColumn = new double[columns];
            double[,] tmpValues = Values;

            for (int i = 0; i < columns; i++)
            {
                tmpColumn[i] = tmpValues[i, first];
            }

            for (int i = 0; i < columns; i++)
            {
                tmpValues[i, first] = tmpValues[i, second];
            }
            for (int i = 0; i < columns; i++)
            {
                tmpValues[i, second] = tmpColumn[i];
            }
            returnMatrix.FillMatrix(tmpValues);

            return returnMatrix;
        }

        public Matrix AddMatrix(Matrix secondMatrix)
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[,] tmpValues = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tmpValues[i, j] = Values[i, j] + secondMatrix.Values[i, j];
                }
            }
            returnMatrix.FillMatrix(tmpValues);

            return returnMatrix;
        }

        public Matrix SubtractMatrix(Matrix secondMatrix)
        {
            Matrix returnMatrix = new Matrix(rows, columns);
            double[,] tmpValues = new double[rows, columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    tmpValues[i, j] = Values[i, j] - secondMatrix.Values[i, j];
                }
            }
            returnMatrix.FillMatrix(tmpValues);

            return returnMatrix;
        }

        public Matrix MultiplyMatrices(Matrix secondMatrix)
        {
            Matrix returnMatrix = new Matrix(rows, secondMatrix.Columns);
            double[,] tmpValues = new double[rows, secondMatrix.Columns];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < secondMatrix.Columns; j++)
                {
                    for (int k = 0; k < secondMatrix.Rows; k++)
                    {
                        tmpValues[i, j] += Values[i, k] * secondMatrix.Values[k, j];
                    }
                }
            }
            returnMatrix.FillMatrix(tmpValues);

            return returnMatrix;
        }

        public bool CompareMatrices(Matrix secondMatrix)
        {
            bool result = true;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    if (Values[i, j] != secondMatrix.Values[i, j])
                    {
                        result = false;
                        break;
                    }
                }
            }

            return result;
        }

        public string SaveString()
        {
            string saveToFile = string.Empty;

            saveToFile += Convert.ToString(rows) + ";" + Convert.ToString(columns);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    saveToFile += ";" + Convert.ToString(Values[i, j]);
                }
            }

            return saveToFile;
        }

        public void ShowMatrixDebug()
        {
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    Debug.Write(values[i, j]);
                }
                Debug.WriteLine("");
            }
        }
    }
}
