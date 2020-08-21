using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Globalization;

namespace MatrixCalculatorWPF
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static string version = "1.0";
        static List<Matrix> matrixList = new List<Matrix>();
        static List<RadioButton> OneMatrixOperation = new List<RadioButton>();
        static List<RadioButton> TwoMatrixOperation = new List<RadioButton>();
        static List<ComboBox> indexLists = new List<ComboBox>();
        static List<object> languageList = new List<object>();

        static bool wasSaved = false;

        static int modifySelectedMatrix  = - 1;
        static int analyzeSelectedMatrix = -1;
        static int firstVieMatrixIndex = -1;
        static int secondVieMatrixIndex = -1;
        static int operationFirstMatrixIndex = -1;
        static int operationSecondMatrixIndex = -1;

        static string trueString = "True";
        static string falseString = "False";
        static string errorMSG = "ERROR";
        static string infoMSG = "INFORMATION";
        static string warningMSG = "WARNING";
        static string questionMSG = "QUESTION";
        static string errorInPos = "Incorrect value in position: ";
        static string addedNewMatrix = "Successfully added new matrix!";
        static string modifiedMatrix = "Successfully modified matrix!";
        static string matrixOrderString = " (ORDER: ";
        static string isSingularString = ", singular.";
        static string notSingularString = ", not singular.";
        static string notSquareString = "Matrix is not square!";
        static string triangularLOWandUPString = "LOW and UP";
        static string triangularLOWString = "LOW";
        static string triangularUPString = "UP";
        static string cannotOverwriteString = "Cannot overwrite";
        static string tooBigMatrixString = "Matrix is too big!";
        static string biggerDimensionsString = "Dimensions must be bigger than 0!";
        static string ruSureString = "Are you sure?";
        static string incorrectInputString = "Incorrect input!";
        static string incorrectMatrixString = "Incorrect matrix!";
        static string incorrectScalarString = "Incorrect scalar value!";
        static string incorrectColumnsString = "Incorrect columns values!";
        static string incorrectRowsString = "Incorrect rows values!";
        static string incorrectMatricesString = "Incorrect matrices!";
        static string notEqualDimensionsString = "Matrices' dimensions are not equal! ";
        static string viceVersaString = "\nTry vice versa.";
        static string differentMatricesString = "Matrices are different.";
        static string theSameString = "The first matrix is the same as the second.";
        static string savedString = "Succesfully saved matrices! (SAVED: ";
        static string loadedString = "Succesfully loaded matrices! (LOADED: ";
        static string cannotSave = "Cannot save matrices. (No matrices to save)";
        static string cannotLoad = "Cannot load matrices.";
        static string loadOption = "Create new list? (if no - matrices will be add to existing list)";
        static string wantSave = "Do you want to save your list?";

        enum ShowMethod
        {
            Modify,
            View
        }

        enum CreateMethod
        {
            Clear,
            Identity
        }

        public MainWindow()
        {
            InitializeComponent();

            tbVersionValue.Text = version;
            tbAuthorValue.Text = "Jacek Jendrzejewski";

            bAddMatrix.IsEnabled = false;
            lvErrors.Opacity = 0.0;
            lvErrorsModify.Opacity = 0.0;

            indexLists.Add(cbModifyMatrix);
            indexLists.Add(cbViewMatrixOne);
            indexLists.Add(cbViewMatrixTwo);
            indexLists.Add(cbAnalyzeMatrix);
            indexLists.Add(cbOperationFirstMatrix);
            indexLists.Add(cbOperationSecondMatrix);

            OneMatrixOperation.Add(rbTranspose);
            OneMatrixOperation.Add(rbCofactor);
            OneMatrixOperation.Add(rbInverse);
            OneMatrixOperation.Add(rbMultiplyByNumber);
            OneMatrixOperation.Add(rbReplaceTwoColumns);
            OneMatrixOperation.Add(rbReplaceTwoRows);
            OneMatrixOperation.Add(rbOverwrite);

            TwoMatrixOperation.Add(rbAddMatrices);
            TwoMatrixOperation.Add(rbSubtractMatrices);
            TwoMatrixOperation.Add(rbMultiplyMatrices);
            TwoMatrixOperation.Add(rbCompareMatrices);

            foreach (RadioButton radioButton in OneMatrixOperation)
            {
                radioButton.IsEnabled = false;
            }
            foreach (RadioButton radioButton in TwoMatrixOperation)
            {
                radioButton.IsEnabled = false;
            }


            tbColumnOne.IsEnabled = false;
            tbColumnTwo.IsEnabled = false;

            tbRowOne.IsEnabled = false;
            tbRowTwo.IsEnabled = false;

            tbScalar.IsEnabled = false;

            if (CultureInfo.CurrentCulture.Name == "pl-PL")
            {
                ChangeToPolish();
                cbChangeLanguage.SelectedIndex = 1;
            }
        }

        //private void ChangeColors(TabItem item)
        //{
        //    Brush initColor = item.Background;
        //    SolidColorBrush selectedColor = (SolidColorBrush)initColor;

        //    const byte diff = 20;
        //    Color newColor = new Color();
        //    newColor.R = (byte)(selectedColor.Color.R - diff);
        //    newColor.G = (byte)(selectedColor.Color.G - diff);
        //    newColor.B = (byte)(selectedColor.Color.B - diff);

        //    selectedColor = new SolidColorBrush(newColor);

        //    if (item.IsSelected)
        //    {
        //        item.Background = selectedColor;
        //        item.Header = "SELECTED";

        //    }
        //    else
        //    {
        //        item.Background = initColor;
        //        item.Header = "UNSELECTED";
        //    }
        //}

        private void CreateMatrixGrid(int rows, int columns, CreateMethod method)
        {
            tbAuthorValue.Text = "Jacek Jendrzejewski";
            lvErrors.ItemsSource = null;
            lvErrors.Items.Clear();
            lvErrors.Opacity = 0.0;
            gMatrix.Children.Clear();
            gMatrix.RowDefinitions.Clear();
            gMatrix.ColumnDefinitions.Clear();

            for (int i = 0; i < rows; i++)
            {
                gMatrix.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns; i++)
            {
                gMatrix.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int more = rows;
            if (columns > rows)
                more = columns;

            double size = 100 - more * 4.25;

            double fontSize = 36;
            if (more > 3)
                fontSize = -1.8 * more + 41;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TextBox tb = new TextBox();
                    tb.Width = size;
                    tb.Height = size;
                    tb.FontSize = fontSize;
                    tb.HorizontalContentAlignment = HorizontalAlignment.Center;
                    tb.VerticalContentAlignment = VerticalAlignment.Center;
                    if (method == CreateMethod.Identity)
                    {
                        if (i == j)
                            tb.Text = "1";
                        else
                            tb.Text = "0";
                    }
                    gMatrix.Children.Add(tb);
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j);
                }
            }
        }

        private void CreateMatrix()
        {
            lvErrors.ItemsSource = null;
            lvErrors.Items.Clear();
            lvErrors.Opacity = 0.0;

            bool errorExist = false;

            List<string> errorList = new List<string>();

            int rows = gMatrix.RowDefinitions.Count();
            int columns = gMatrix.ColumnDefinitions.Count();

            double[,] matrix = new double[rows, columns];

            int n = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    try
                    {
                        string text = (gMatrix.Children[n] as TextBox).Text;
                        if (text == "" && (cbFillEmpty.IsChecked == true))
                            text = "0";
                        double number = Convert.ToDouble(text);
                        matrix[i, j] = number;
                        (gMatrix.Children[n] as TextBox).Background = Brushes.MediumSeaGreen;
                    }
                    catch
                    {
                        if (!errorExist)
                        {
                            errorExist = true;
                        }

                        errorList.Add(errorInPos + (i + 1) + ", " + (j + 1));
                        (gMatrix.Children[n] as TextBox).Background = Brushes.PaleVioletRed;
                    }
                    n++;
                }
            }

            if (errorExist)
            {
                lvErrors.Opacity = 1.0;
                lvErrors.ItemsSource = errorList;
            }
            else
            {
                matrixList.Add(new Matrix(rows, columns));
                matrixList[matrixList.Count - 1].FillMatrix(matrix);
                foreach (ComboBox comboBox in indexLists)
                {
                    comboBox.Items.Add(matrixList.Count);
                }
                wasSaved = false;
                MessageBox.Show(addedNewMatrix, infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ModifyMatrix(int matrixIndex)
        {
            lvErrorsModify.ItemsSource = null;
            lvErrorsModify.Items.Clear();
            lvErrorsModify.Opacity = 0.0;

            bool errorExist = false;

            List<string> errorList = new List<string>();

            int rows = matrixList[matrixIndex].Rows;
            int columns = matrixList[matrixIndex].Columns;

            double[,] matrix = new double[rows, columns];

            int n = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    try
                    {
                        string text = (gMatrixModify.Children[n] as TextBox).Text;
                        double number = Convert.ToDouble(text);
                        matrix[i, j] = number;
                        (gMatrixModify.Children[n] as TextBox).Background = Brushes.MediumSeaGreen;
                    }
                    catch
                    {
                        if (!errorExist)
                        {
                            errorExist = true;
                        }

                        errorList.Add(errorInPos + (i + 1) + ", " + (j + 1));
                        (gMatrixModify.Children[n] as TextBox).Background = Brushes.PaleVioletRed;
                    }
                    n++;
                }
            }

            if (errorExist)
            {
                lvErrorsModify.Opacity = 1.0;
                lvErrorsModify.ItemsSource = errorList;
            }
            else
            {
                matrixList[matrixIndex].FillMatrix(matrix);
                matrixList[matrixIndex].WasAnalyzed = false;
                wasSaved = false;
                MessageBox.Show(modifiedMatrix, infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AnalyzeMatrix(int matrixIndex)
        {
            Matrix tmp = matrixList[matrixIndex];

            if (!tmp.WasAnalyzed)
                tmp.Analyze();

            Brush falseBrush = Brushes.IndianRed;
            Brush trueBrush = Brushes.LightGreen;

            if (tmp.IsSquare)
            {
                tbSquare.Text = trueString + matrixOrderString + tmp.MatrixOrder + " )";
                tbSquare.Foreground = trueBrush;
                matrixList[matrixIndex].TraceValue = tmp.TraceValue;
                tbTrace.Text = Convert.ToString(tmp.TraceValue);
                matrixList[matrixIndex].Determinant = tmp.Determinant;
                tbDeterminant.Text = Convert.ToString(tmp.Determinant);
                if (tmp.IsSingular)
                    tbDeterminant.Text += isSingularString;
                else
                    tbDeterminant.Text += notSingularString;
            }
            else
            {
                tbSquare.Text = falseString;
                tbTrace.Text = notSquareString;
                tbDeterminant.Text = notSquareString;
                tbSquare.Foreground = falseBrush;
                tbTrace.Foreground = falseBrush;
                tbDeterminant.Foreground = falseBrush;
            }
            if (tmp.IsZero)
            {
                tbZero.Text = trueString;
                tbZero.Foreground = trueBrush;
            }
            else
            {
                tbZero.Text = falseString;
                tbZero.Foreground = falseBrush;
            }
            if (tmp.IsIdentity)
            {
                tbIdentity.Text = trueString;
                tbIdentity.Foreground = trueBrush;
            }
            else
            {
                tbIdentity.Text = falseString;
                tbIdentity.Foreground = falseBrush;
            }
            if (tmp.IsDiagonal)
            {
                tbDiagonal.Text = trueString;
                tbDiagonal.Foreground = trueBrush;
            }
            else
            {
                tbDiagonal.Text = falseString;
                tbDiagonal.Foreground = falseBrush;
            }
            if (tmp.IsTriangularLOW && tmp.IsTriangularUP)
            {
                tbTriangular.Foreground = trueBrush;
                tbTriangular.Text = triangularLOWandUPString;
            }
            else if (tmp.IsTriangularLOW && !tmp.IsTriangularUP)
            {
                tbTriangular.Foreground = trueBrush;
                tbTriangular.Text = triangularLOWString;
            }
            else if (!tmp.IsTriangularLOW && tmp.IsTriangularUP)
            {
                tbTriangular.Foreground = trueBrush;
                tbTriangular.Text = triangularUPString;
            }
            else if (!tmp.IsTriangularLOW && !tmp.IsTriangularUP)
            {
                tbTriangular.Text = falseString;
                tbTriangular.Foreground = falseBrush;
            }
            if (tmp.IsSymmetric)
            {
                tbSymmetric.Text = trueString;
                tbSymmetric.Foreground = trueBrush;
            }
            else
            {
                tbSymmetric.Text = falseString;
                tbSymmetric.Foreground = falseBrush;
            }
        }

        private void ShowMatrix(int matrixIndex, Grid area, ShowMethod showMethod)
        {
            area.Children.Clear();
            area.RowDefinitions.Clear();
            area.ColumnDefinitions.Clear();

            Matrix shownMatrix = matrixList[matrixIndex];
            int rows = shownMatrix.Rows;
            int columns = shownMatrix.Columns;

            for (int i = 0; i < rows; i++)
            {
                area.RowDefinitions.Add(new RowDefinition());
            }

            for (int i = 0; i < columns; i++)
            {
                area.ColumnDefinitions.Add(new ColumnDefinition());
            }

            int more = rows;
            if (columns > rows)
                more = columns;

            double size = 100 - more * 4.25;

            double fontSize = 36;
            if (more > 3)
                fontSize = -1.8 * more + 41;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    TextBox tb = new TextBox();
                    tb.Width = size;
                    tb.Height = size;
                    tb.FontSize = fontSize;
                    tb.HorizontalContentAlignment = HorizontalAlignment.Center;
                    tb.VerticalContentAlignment = VerticalAlignment.Center;

                    if (Math.Abs(shownMatrix.Values[i, j] - Math.Round(shownMatrix.Values[i, j])) > 0)
                        tb.Text = string.Format("{0:F2}", shownMatrix.Values[i, j]);
                    else
                        tb.Text = string.Format("{0}", shownMatrix.Values[i, j]);

                    if (showMethod == ShowMethod.View)
                    {
                        tb.Background = null;
                        tb.IsEnabled = false;
                    }
                    area.Children.Add(tb);
                    Grid.SetRow(tb, i);
                    Grid.SetColumn(tb, j);
                }
            }
        }

        private int GetMatrixIndex(object sender)
        {
            return (sender as ComboBox).SelectedIndex;
        }

        private void ClearMatrix(Grid matrix)
        {
            matrix.Children.Clear();
            matrix.RowDefinitions.Clear();
            matrix.ColumnDefinitions.Clear();
        }

        private void ClearResult()
        {
            tbSquare.Text = string.Empty;
            tbZero.Text = string.Empty;
            tbDiagonal.Text = string.Empty;
            tbIdentity.Text = string.Empty;
            tbTriangular.Text = string.Empty;
            tbSymmetric.Text = string.Empty;
            tbTrace.Text = string.Empty;
            tbDeterminant.Text = string.Empty;
        }

        private void CompleteCalculation(Matrix result)
        {
            if ((bool)rbYes.IsChecked)
            {
                matrixList.Add(result);
                foreach (ComboBox comboBox in indexLists)
                {
                    comboBox.Items.Add(matrixList.Count);
                }
            }
            if ((bool)rbOverwrite.IsChecked)
            {
                if (operationFirstMatrixIndex >= 0 && operationFirstMatrixIndex < matrixList.Count)
                {
                    matrixList[operationFirstMatrixIndex] = result;
                    matrixList[operationFirstMatrixIndex].WasAnalyzed = false;
                }
                else
                    MessageBox.Show(cannotOverwriteString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
            }

            matrixList.Add(result);
            ShowMatrix(matrixList.Count - 1, gOperationMatrix, ShowMethod.View);
            matrixList.RemoveAt(matrixList.Count - 1);
        }

        private void bCreateMatrix_Click(object sender, RoutedEventArgs e)
        {
            CreateMethod cMethod;

            if ((sender as Button).Name == "bCreateIdentityMatrix")
                cMethod = CreateMethod.Identity;
            else
                cMethod = CreateMethod.Clear;

            if (int.TryParse(tbHeight.Text, out int rows) && int.TryParse(tbWidth.Text, out int columns))
            {
                if (rows >= 1 && rows <= 18 && columns >= 1 && columns <= 18)
                {
                    if (cMethod == CreateMethod.Identity && rows != columns)
                    {
                        bAddMatrix.IsEnabled = false;
                        MessageBox.Show(notSquareString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else if ((cMethod == CreateMethod.Identity && rows == columns) || cMethod == CreateMethod.Clear)
                    {
                        CreateMatrixGrid(rows, columns, cMethod);
                        bAddMatrix.IsEnabled = true;
                    }
                }
                else if (rows > 18 || columns > 18)
                {
                    bAddMatrix.IsEnabled = false;
                    MessageBox.Show(tooBigMatrixString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (rows < 1 || columns < 1)
                {
                    bAddMatrix.IsEnabled = false;
                    MessageBox.Show(biggerDimensionsString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                bAddMatrix.IsEnabled = false;
                MessageBox.Show(incorrectInputString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void bAddMatrix_Click(object sender, RoutedEventArgs e)
        {
            CreateMatrix();
        }

        private void bModify_Click(object sender, RoutedEventArgs e)
        {
            if (modifySelectedMatrix >= 0 && modifySelectedMatrix < matrixList.Count)
            {
                ModifyMatrix(modifySelectedMatrix);
                if (cbAnalyzeMatrix.SelectedIndex == cbModifyMatrix.SelectedIndex && cbAnalyzeMatrix.SelectedIndex != -1)
                    ShowMatrix(cbAnalyzeMatrix.SelectedIndex, gAnalyzeMatrix, ShowMethod.View);
            }
            else
                MessageBox.Show(incorrectMatrixString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
            tbAuthorValue.Text = "Jacek Jendrzejewski";
        }

        private void bDelete_Click(object sender, RoutedEventArgs e)
        {

            if (modifySelectedMatrix >= 0 && modifySelectedMatrix < matrixList.Count)
            {
                MessageBoxResult result = MessageBox.Show(ruSureString, warningMSG, MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    matrixList.RemoveAt(modifySelectedMatrix);

                    foreach (ComboBox comboBox in indexLists)
                    {
                        comboBox.Items.RemoveAt(modifySelectedMatrix);
                    }

                    for (int i = 0; i < cbModifyMatrix.Items.Count; i++)
                    {
                        foreach (ComboBox comboBox in indexLists)
                        {
                            comboBox.Items[i] = i + 1;
                        }
                    }
                    wasSaved = false;
                    modifySelectedMatrix = -1;
                    lvErrorsModify.Opacity = 0.0;
                }
            }
            else
                MessageBox.Show(incorrectMatrixString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void bRefresh_Click(object sender, RoutedEventArgs e)
        {
            lvErrorsModify.Opacity = 0.0;
            if (modifySelectedMatrix >= 0 && modifySelectedMatrix < matrixList.Count)
                ShowMatrix(modifySelectedMatrix, gMatrixModify, ShowMethod.Modify);
        }

        private void bAnalyze_Click(object sender, RoutedEventArgs e)
        {
            if (analyzeSelectedMatrix >= 0 && analyzeSelectedMatrix < matrixList.Count && cbAnalyzeMatrix.SelectedIndex != -1)
                AnalyzeMatrix(analyzeSelectedMatrix);
            else
                MessageBox.Show(incorrectMatrixString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void lvErrors_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            string error;
            int rowPos, columnPos;

            var item = sender as ListViewItem;
            error = Convert.ToString(item.Content);
            string[] tmp = error.Split(':')[1].Split(',');

            int.TryParse(tmp[0], out rowPos);
            int.TryParse(tmp[1], out columnPos);

            TextBox tb = gMatrix.Children[(rowPos - 1) * gMatrix.ColumnDefinitions.Count + (columnPos - 1)] as TextBox;
            tb.Background = Brushes.Red;
        }

        private void cbViewMatrix_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int mIndex = GetMatrixIndex(sender);
            ComboBox tmpCB = sender as ComboBox;
            Grid tmpG = new Grid();
            ShowMethod tmpSM = ShowMethod.View;

            if (tmpCB.Name == "cbViewMatrixOne")
            {
                tmpG = gMatrixViewOne;
                if (mIndex != -1)
                    firstVieMatrixIndex = mIndex;
            }
            else if (tmpCB.Name == "cbViewMatrixTwo")
            {
                tmpG = gMatrixViewTwo;
                if (mIndex != -1)
                    secondVieMatrixIndex = mIndex;
            }
            else if (tmpCB.Name == "cbModifyMatrix")
            {
                lvErrorsModify.Opacity = 0.0;
                tmpG = gMatrixModify;
                tmpSM = ShowMethod.Modify;
                if (mIndex != -1)
                    modifySelectedMatrix = mIndex;
            }
            else if (tmpCB.Name == "cbAnalyzeMatrix")
            {
                tmpG = gAnalyzeMatrix;
                ClearResult();
                analyzeSelectedMatrix = mIndex;
            }
            else if (tmpCB.Name == "cbOperationFirstMatrix")
            {
                operationFirstMatrixIndex = mIndex;
                return;
            }
            else if (tmpCB.Name == "cbOperationSecondMatrix")
            {
                operationSecondMatrixIndex = mIndex;
                return;
            }

            if (mIndex >= 0 && mIndex < matrixList.Count)
            {
                ShowMatrix(mIndex, tmpG, tmpSM);
                if (tmpCB.Name == "cbAnalyzeMatrix")
                    analyzeSelectedMatrix = mIndex;
            }
            else
                ClearMatrix(tmpG);
        }

        private void rbOneMatrix_Checked(object sender, RoutedEventArgs e)
        {

            foreach (RadioButton radioButton in OneMatrixOperation)
            {
                radioButton.IsEnabled = true;
                radioButton.Foreground = Brushes.Black;
            }

            tbSecondMatrix.IsEnabled = false;
            tbSecondMatrix.Foreground = Brushes.DimGray;
            cbOperationSecondMatrix.IsEnabled = false;


            foreach (RadioButton radioButton in TwoMatrixOperation)
            {
                radioButton.IsEnabled = false;
                radioButton.IsChecked = false;
                radioButton.Foreground = Brushes.DimGray;
            }
            tbAuthorValue.Text = "Jacek Jendrzejewski";
        }

        private void rbTwoMatrices_Checked(object sender, RoutedEventArgs e)
        {
            foreach (RadioButton radioButton in OneMatrixOperation)
            {
                radioButton.IsEnabled = false;
                radioButton.IsChecked = false;
                radioButton.Foreground = Brushes.DimGray;
            }
            tbColumnOne.IsEnabled = false;
            tbColumnTwo.IsEnabled = false;
            tbRowOne.IsEnabled = false;
            tbRowTwo.IsEnabled = false;
            tbScalar.IsEnabled = false;

            tbSecondMatrix.IsEnabled = true;
            tbSecondMatrix.Foreground = Brushes.Black;
            cbOperationSecondMatrix.IsEnabled = true;

            foreach (RadioButton radioButton in TwoMatrixOperation)
            {
                radioButton.IsEnabled = true;
                radioButton.Foreground = Brushes.Black;
            }
        }

        private void bCalcOperation_Click(object sender, RoutedEventArgs e)
        {
            wasSaved = false;
            int firstIndex = cbOperationFirstMatrix.SelectedIndex;
            int secondIndex = cbOperationSecondMatrix.SelectedIndex;

            Matrix result;

            if (firstIndex >= 0 && firstIndex < cbOperationFirstMatrix.Items.Count && (bool)rbOneMatrix.IsChecked)
            {

                if ((bool)rbTranspose.IsChecked)
                {
                    result = matrixList[firstIndex].TransposeMatrix();
                    CompleteCalculation(result);
                }
                else if ((bool)rbCofactor.IsChecked)
                {
                    if (!matrixList[firstIndex].WasAnalyzed)
                        matrixList[firstIndex].Analyze();

                    if (matrixList[firstIndex].IsSquare)
                    {
                        result = matrixList[firstIndex].InverseMatrix();
                        CompleteCalculation(result);
                    }
                    else
                        MessageBox.Show(notSquareString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if ((bool)rbInverse.IsChecked)
                {
                    if (!matrixList[firstIndex].WasAnalyzed)
                        matrixList[firstIndex].Analyze();

                    if (matrixList[firstIndex].IsSquare)
                    {
                        if (!matrixList[firstIndex].IsSingular)
                        {
                            result = matrixList[firstIndex].InverseMatrix();
                            CompleteCalculation(result);
                        }
                        else
                            MessageBox.Show(isSingularString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show(notSquareString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);

                }
                else if ((bool)rbMultiplyByNumber.IsChecked)
                {
                    if (double.TryParse(tbScalar.Text, out double scalar))
                    {
                        result = matrixList[firstIndex].MultiplyByNumber(scalar);
                        CompleteCalculation(result);
                    }
                    else
                        MessageBox.Show(incorrectScalarString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if ((bool)rbReplaceTwoColumns.IsChecked)
                {
                    if (int.TryParse(tbColumnOne.Text, out int columnOne) && int.TryParse(tbColumnTwo.Text, out int columnTwo))
                    {
                        int columns = matrixList[firstIndex].Columns;
                        if (columnOne > 0 && columnOne <= columns && columnTwo > 0 && columnTwo <= columns)
                        {
                            result = matrixList[firstIndex].ReplaceTwoColumns(columnOne - 1, columnTwo - 1);
                            CompleteCalculation(result);
                        }
                        else
                            MessageBox.Show(incorrectColumnsString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show(incorrectColumnsString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if ((bool)rbReplaceTwoRows.IsChecked)
                {
                    if (int.TryParse(tbRowOne.Text, out int rowOne) && int.TryParse(tbRowTwo.Text, out int rowTwo) && (bool)rbTwoMatrices.IsChecked)
                    {
                        int rows = matrixList[firstIndex].Rows;
                        if (rowOne > 0 && rowOne <= rows && rowTwo > 0 && rowTwo <= rows)
                        {
                            result = matrixList[firstIndex].ReplaceTwoRows(rowOne - 1, rowTwo - 1);
                            CompleteCalculation(result);
                        }
                        else
                            MessageBox.Show(incorrectRowsString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                        MessageBox.Show(incorrectRowsString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else if (firstIndex >= 0 && firstIndex < cbOperationFirstMatrix.Items.Count && secondIndex >= 0 && secondIndex < cbOperationSecondMatrix.Items.Count)
            {
                Matrix firstMatrix = matrixList[firstIndex];
                Matrix secondMatrix = matrixList[secondIndex];
                if ((bool)rbAddMatrices.IsChecked)
                {
                    if (firstMatrix.Rows == secondMatrix.Rows && firstMatrix.Columns == secondMatrix.Columns)
                    {
                        result = firstMatrix.AddMatrix(secondMatrix);
                        CompleteCalculation(result);
                    }
                    else
                    {
                        string dimension = string.Format("{0}x{1} != {2}x{3}", firstMatrix.Rows, firstMatrix.Columns, secondMatrix.Rows, secondMatrix.Columns);
                        MessageBox.Show(notEqualDimensionsString + dimension, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if ((bool)rbSubtractMatrices.IsChecked)
                {
                    if (firstMatrix.Rows == secondMatrix.Rows && firstMatrix.Columns == secondMatrix.Columns)
                    {
                        result = firstMatrix.SubtractMatrix(secondMatrix);
                        CompleteCalculation(result);
                    }
                    else
                    {
                        string dimension = string.Format("{0}x{1} != {2}x{3}", firstMatrix.Rows, firstMatrix.Columns, secondMatrix.Rows, secondMatrix.Columns);
                        MessageBox.Show(notEqualDimensionsString + dimension, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if ((bool)rbMultiplyMatrices.IsChecked)
                {
                    if (firstMatrix.Columns == secondMatrix.Rows)
                    {
                        result = firstMatrix.MultiplyMatrices(secondMatrix);
                        CompleteCalculation(result);
                    }
                    else if (firstMatrix.Rows == secondMatrix.Columns)
                    {
                        string dimension = string.Format("{0}x{1} != {2}x{3}", firstMatrix.Rows, firstMatrix.Columns, secondMatrix.Rows, secondMatrix.Columns);
                        MessageBox.Show(notEqualDimensionsString + dimension + viceVersaString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        string dimension = string.Format("{0}x{1} != {2}x{3}", firstMatrix.Rows, firstMatrix.Columns, secondMatrix.Rows, secondMatrix.Columns);
                        MessageBox.Show(notEqualDimensionsString + dimension, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else if ((bool)rbCompareMatrices.IsChecked)
                {
                    if (firstMatrix.Rows == secondMatrix.Rows && firstMatrix.Columns == secondMatrix.Columns)
                    {
                        bool comparisonResult = firstMatrix.CompareMatrices(secondMatrix);
                        if (comparisonResult)
                            MessageBox.Show(theSameString, infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
                        else
                            MessageBox.Show(differentMatricesString, infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        string dimension = string.Format("{0}x{1} != {2}x{3}", firstMatrix.Rows, firstMatrix.Columns, secondMatrix.Rows, secondMatrix.Columns);
                        MessageBox.Show(notEqualDimensionsString + dimension, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            else
                MessageBox.Show(incorrectMatricesString, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);

        }
        private void rbReplaceTwoRows_Checked(object sender, RoutedEventArgs e)
        {
            tbRowOne.IsEnabled = true;
            tbRowTwo.IsEnabled = true;
        }
        private void rbReplaceTwoRows_Unchecked(object sender, RoutedEventArgs e)
        {
            tbRowOne.IsEnabled = false;
            tbRowTwo.IsEnabled = false;
        }
        private void rbReplaceTwoColumns_Checked(object sender, RoutedEventArgs e)
        {
            tbColumnOne.IsEnabled = true;
            tbColumnTwo.IsEnabled = true;
        }
        private void rbReplaceTwoColumns_Unchecked(object sender, RoutedEventArgs e)
        {
            tbColumnOne.IsEnabled = false;
            tbColumnTwo.IsEnabled = false;
        }
        private void rbMultiplyByNumber_Checked(object sender, RoutedEventArgs e)
        {
            tbScalar.IsEnabled = true;
        }
        private void rbMultiplyByNumber_Unchecked(object sender, RoutedEventArgs e)
        {
            tbScalar.IsEnabled = false;
        }
        private void rbCompareMatrices_Checked(object sender, RoutedEventArgs e)
        {
            tbAddToList.Foreground = Brushes.DimGray;
            rbYes.Foreground = Brushes.DimGray;
            rbNo.Foreground = Brushes.DimGray;
            rbOverwrite.Foreground = Brushes.DimGray;
            cSaveOptions.IsEnabled = false;
        }
        private void rbCompareMatrices_Unchecked(object sender, RoutedEventArgs e)
        {
            cSaveOptions.IsEnabled = true;
            tbAddToList.Foreground = Brushes.Black;
            rbYes.Foreground = Brushes.Black;
            rbNo.Foreground = Brushes.Black;
            rbOverwrite.Foreground = Brushes.Black;
        }
        private void cbChangeLanguage_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 0 - English
            // 1 - Polish
            ComboBox cbTMP = sender as ComboBox;

            if (tiSet.IsSelected)
            {
                if (cbTMP.SelectedIndex == 0)
                {
                    ChangeToEnglish();
                    Debug.WriteLine("Zmieniam język!");
                }
                else if (cbTMP.SelectedIndex == 1)
                {
                    ChangeToPolish();
                    Debug.WriteLine("Zmieniam język!");
                }
            }
        }
        private void bSaveMatrices_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text file (*.csv)|*.csv";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);


            if (matrixList.Count > 0)
            {
                if ((bool)saveFileDialog.ShowDialog())
                {
                    string saveDirectory = saveFileDialog.FileName;
                    using (StreamWriter sw = new StreamWriter(saveDirectory))
                    {
                        sw.WriteLine("Rows;Columns;Content");
                        foreach (Matrix matrix in matrixList)
                        {
                            sw.WriteLine(matrix.SaveString());
                        }
                    }
                    wasSaved = true;
                    MessageBox.Show(savedString + Convert.ToString(matrixList.Count) + ")", infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
                MessageBox.Show(cannotSave, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);

        }
        private void bOpenMatrices_Click(object sender, RoutedEventArgs e)
        {
            string readDirectory;
            int listCount = matrixList.Count;

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.csv)|*.csv|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                MessageBoxResult boxResult = MessageBox.Show(loadOption, questionMSG, MessageBoxButton.YesNo, MessageBoxImage.Question);
                readDirectory = openFileDialog.FileName;

                using (StreamReader sr = new StreamReader(readDirectory))
                {
                    string heading = sr.ReadLine();
                    string line;

                    if (boxResult == MessageBoxResult.Yes)
                    {
                        matrixList.Clear();
                        foreach (ComboBox comboBox in indexLists)
                        {
                            comboBox.Items.Clear();
                        }
                        listCount = matrixList.Count;
                    }

                    do
                    {
                        line = sr.ReadLine();

                        if (line == null)
                            break;

                        string[] data = line.Split(';');

                        int.TryParse(data[0], out int rows);
                        int.TryParse(data[1], out int columns);
                        double[,] tmpValues = new double[rows, columns];

                        int k = 2;
                        for (int i = 0; i < rows; i++)
                        {
                            for (int j = 0; j < columns; j++)
                            {
                                tmpValues[i, j] = Convert.ToDouble(data[k]);
                                k++;
                            }
                        }

                        matrixList.Add(new Matrix(rows, columns));
                        matrixList[matrixList.Count - 1].FillMatrix(tmpValues);
                        foreach (ComboBox comboBox in indexLists)
                        {
                            comboBox.Items.Add(matrixList.Count);
                        }
                    } while (true);

                }

                MessageBox.Show(loadedString + Convert.ToString(matrixList.Count - listCount) + ")", infoMSG, MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show(cannotLoad, errorMSG, MessageBoxButton.OK, MessageBoxImage.Error);


        }
        private void bExit_Click(object sender, RoutedEventArgs e)
        {
            if (!wasSaved && matrixList.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show(wantSave, questionMSG, MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    bSaveMatrices_Click(sender, e);
                }
                else if (result == MessageBoxResult.No)
                {
                    Close();
                }
            }
            else
                Close();
        }
        private void ChangeToEnglish()
        {
            trueString = "True";
            falseString = "False";
            errorMSG = "ERROR";
            infoMSG = "INFORMATION";
            warningMSG = "WARNING";
            errorInPos = "Incorrect value in position: ";
            addedNewMatrix = "Successfully added new matrix!";
            modifiedMatrix = "Successfully modified matrix!";
            matrixOrderString = " (ORDER: ";
            isSingularString = ", singular.";
            notSingularString = ", not singular.";
            notSquareString = "Matrix is not square!";
            triangularLOWandUPString = "LOW and UP";
            triangularLOWString = "LOW";
            triangularUPString = "UP";
            cannotOverwriteString = "Cannot overwrite.";
            tooBigMatrixString = "Matrix is too big!";
            biggerDimensionsString = "Dimensions must be bigger than 0!";
            ruSureString = "Are you sure?";
            incorrectInputString = "Incorrect input!";
            incorrectMatrixString = "Incorrect matrix!";
            incorrectScalarString = "Incorrect scalar value!";
            incorrectColumnsString = "Incorrect columns values!";
            incorrectRowsString = "Incorrect rows values!";
            incorrectMatricesString = "Incorrect matrices!";
            notEqualDimensionsString = "Matrices' dimensions are not equal! ";
            viceVersaString = "\nTry vice versa.";
            differentMatricesString = "Matrices are different.";
            theSameString = "Matrices are identical.";
            savedString = "Succesfully saved matrices! (SAVED: ";
            loadedString = "Succesfully loaded matrices! (LOADED: ";
            loadOption = "Create new list? (if no - matrices will be add to existing list)";
            cannotSave = "Cannot save matrices. (No matrices to save)";
            cannotLoad = "Cannot load matrices.";

            hAddMatrix.Text = "Add matrix";
            hModifyMatrix.Text = "Modify matrix";
            hViewMatrix.Text = "View matrices";
            hCalcMatrix.Text = "Analysis";
            hOperations.Text = "Operations";
            hSettings.Text = "Settings";

            hAddMatrix.ToolTip = "Add new matrix to colection";
            hModifyMatrix.ToolTip = "Modify or delete matrix";
            hViewMatrix.ToolTip = "View your matrices";
            hCalcMatrix.ToolTip = "Matrix properties";
            hOperations.ToolTip = "Matrix operations";
            hSettings.ToolTip = "Change language, save matrices";

            tbAddEnterDimensions.Text = "Enter dimensions(rows X columns)";
            tbAddCreateIdentity.Text = "Create identity matrix";
            tbAddCreateClear.Text = "Create clear matrix";
            tbAddInfo.Text = "INFO: \n > Max 18x18";
            ((GridView)lvErrors.View).Columns[0].Header = "Errors";
            cbFillEmpty.Content = "Fill empty slots with 0";
            bAddMatrix.Content = "Add matrix";

            tbModifyMatrixNumber.Text = "Matrix number:";
            tbRefresh.Text = "Use refresh button to reset changes";
            ((GridView)lvErrorsModify.View).Columns[0].Header = "Errors";
            bModify.Content = "Modify";
            bDelete.Content = "Delete matrix";

            tbViewMatrixFirst.Text = "Matrix number:";
            tbViewMatrixSecond.Text = "Matrix number:";

            tbAnalysisMatrixNumber.Text = "Matrix number:";
            bAnalyze.Content = "Analyze";
            lAnalysisInfo.Content = "Bigger matrices can load longer.";
            tbASquare.Text = "Square:";
            tbAZero.Text = "Zero:";
            tbAIdentity.Text = "Identity:";
            tbADiagonal.Text = "Diagonal:";
            tbATriangular.Text = "Triangular:";
            tbASymmetric.Text = "Symmetric:";
            tbATrace.Text = "Trace:";
            tbADeterminant.Text = "Determinant:";

            tbONumber.Text = "Choose number of matrices";
            rbOneMatrix.Content = "One matrix";
            rbTwoMatrices.Content = "Two matrices";
            tbOFirstMatrix.Text = "Choose first matrix:";
            tbSecondMatrix.Text = "Choose second matrix:";
            tbOOperation.Text = "Choose operation";
            rbTranspose.Content = "Transpose";
            rbCofactor.Content = "Cofactor matrix";
            rbInverse.Content = "Inverse";
            rbMultiplyByNumber.Content = "Multiply by number";
            tbScalar.Text = "Scalar";
            rbReplaceTwoColumns.Content = "Replace 2 columns";
            tbColumnOne.Text = "Column 1";
            tbColumnTwo.Text = "Column 2";
            rbReplaceTwoRows.Content = "Replace 2 rows";
            tbRowOne.Text = "Row 1";
            tbRowTwo.Text = "Row 2";
            rbAddMatrices.Content = "Add matrices";
            rbSubtractMatrices.Content = "Subtract matrices";
            rbMultiplyMatrices.Content = "Multiply matrices";
            rbCompareMatrices.Content = "Compare matrices";
            tbAddToList.Text = "Add result matrix to the list?";
            rbYes.Content = "Yes";
            rbNo.Content = "No";
            rbOverwrite.Content = "Overwrite";
            bCalcOperation.Content = "Calculate";
            tbOResult.Text = "Result";

            tbSLanguage.Text = "Language:";
            bSaveMatrices.Content = "Save matrices";
            bOpenMatrices.Content = "Load matrices";
            bExit.Content = "Exit";
            wantSave = "Do you want to save your list?";
            tbVersion.Text = "Version:";
            tbAuthor.Text = "Author:";


            (cbChangeLanguage.Items[0] as ComboBoxItem).Content = "English";
            (cbChangeLanguage.Items[1] as ComboBoxItem).Content = "Polish";
        }
        private void ChangeToPolish()
        {
            trueString = "Prawda";
            falseString = "Fałsz";
            errorMSG = "BŁĄD";
            infoMSG = "INFORMACJA";
            warningMSG = "OSTRZEŻENIE";
            errorInPos = "Nieprawidłowa wartość w miejscu: ";
            addedNewMatrix = "Pomyślnie dodano nową macierz!";
            modifiedMatrix = "Pomyślnie zedytowano nową macierz!";
            matrixOrderString = " (STOPIEŃ: ";
            isSingularString = ", osobliwa.";
            notSingularString = ", nieosobliwa.";
            notSquareString = "Macierz nie jest kwadratowa!";
            triangularLOWandUPString = "DOLNA i GÓRNA";
            triangularLOWString = "DOLNA";
            triangularUPString = "GÓRNA";
            cannotOverwriteString = "Nie można nadpisać.";
            tooBigMatrixString = "Macierz jest za duża!";
            biggerDimensionsString = "Wymiary macierzy muszą być większe od 0!";
            ruSureString = "Czy na pewno?";
            incorrectInputString = "Błędne dane wejściowe!";
            incorrectMatrixString = "Błędna macierz!";
            incorrectScalarString = "Błędna wartość skalara!";
            incorrectColumnsString = "Błędne wartości kolumn!";
            incorrectRowsString = "Błędne wartości wierszy!";
            incorrectMatricesString = "Błędne macierze!";
            notEqualDimensionsString = "Wymiary macierzy nie są sobie równe! ";
            viceVersaString = "\nSpróbuj odwrotnie.";
            differentMatricesString = "Macierze są różne.";
            theSameString = "Macierze są identyczne.";
            savedString = "Pomyślnie zapisano macierze! (ZAPISANO: ";
            loadedString = "Pomyślnie wczytano macierze! (WCZYTANO: ";
            loadOption = "Stworzyć nową listę? (jeśli nie - macierze zostaną dołączone do aktualnej)";
            cannotSave = "Nie można zapisać macierzy. (Brak macierzy do zapisania)";
            cannotLoad = "Nie można otworzyć macierzy.";

            hAddMatrix.Text = "Dodaj macierz";
            hModifyMatrix.Text = "Edytuj macierz";
            hViewMatrix.Text = "Zobacz macierze";
            hCalcMatrix.Text = "Analiza";
            hOperations.Text = "Operacje";
            hSettings.Text = "Ustawienia";

            hAddMatrix.ToolTip = "Dodaj macierz do kolekcji";
            hModifyMatrix.ToolTip = "Edytuj lub usuń macierz";
            hViewMatrix.ToolTip = "Przejrzyj macierze w kolekcji";
            hCalcMatrix.ToolTip = "Właściwości macierzy";
            hOperations.ToolTip = "Operacje na macierzach";
            hSettings.ToolTip = "Zmień język, zapisz macierze";

            tbAddEnterDimensions.Text = "Wprowadź wymiary (wiersze X kolumny)";
            tbAddCreateIdentity.Text = "Stwórz macierz jednostkową";
            tbAddCreateClear.Text = "Stwórz pustą macierz";
            tbAddInfo.Text = "INFO: \n > Max 18x18";
            ((GridView)lvErrors.View).Columns[0].Header = "Błędy";
            cbFillEmpty.Content = "Uzupełnij puste miejsca zerami";
            bAddMatrix.Content = "Dodaj macierz";

            tbModifyMatrixNumber.Text = "Numer macierzy:";
            tbRefresh.Text = "Użyj przycisku odświeżania by zresetować zmiany";
            ((GridView)lvErrorsModify.View).Columns[0].Header = "Błędy";
            bModify.Content = "Edytuj";
            bDelete.Content = "Usuń macierz";

            tbViewMatrixFirst.Text = "Numer macierzy:";
            tbViewMatrixSecond.Text = "Numer macierzy:";

            tbAnalysisMatrixNumber.Text = "Numer macierzy:";
            bAnalyze.Content = "Analizuj";
            lAnalysisInfo.Content = "Większe macierze mogą się ładować dłużej.";
            tbASquare.Text = "Kwadratowa:";
            tbAZero.Text = "Zerowa:";
            tbAIdentity.Text = "Jednostkowa:";
            tbADiagonal.Text = "Diagonalna:";
            tbATriangular.Text = "Trójkątna:";
            tbASymmetric.Text = "Symetryczna:";
            tbATrace.Text = "Ślad:";
            tbADeterminant.Text = "Wyznacznik:";

            tbONumber.Text = "Wybierz ilość macierzy";
            rbOneMatrix.Content = "Jedna macierz";
            rbTwoMatrices.Content = "Dwie macierze";
            tbOFirstMatrix.Text = "Wybierz pierwszą macierz:";
            tbSecondMatrix.Text = "Wybierz drugą macierz:";
            tbOOperation.Text = "Wybierz operacje";
            rbTranspose.Content = "Transponuj";
            rbCofactor.Content = "Macierz dopełnień";
            rbInverse.Content = "Macierz odwrotna";
            rbMultiplyByNumber.Content = "Pomnóż przez liczbę";
            tbScalar.Text = "Skalar";
            rbReplaceTwoColumns.Content = "Zamień 2 kolumny";
            tbColumnOne.Text = "Kolumna 1";
            tbColumnTwo.Text = "Kolumna 2";
            rbReplaceTwoRows.Content = "Zamień 2 wiersze";
            tbRowOne.Text = "Wiersz 1";
            tbRowTwo.Text = "Wiersz 2";
            rbAddMatrices.Content = "Dodaj macierze";
            rbSubtractMatrices.Content = "Odejmij macierze";
            rbMultiplyMatrices.Content = "Pomnóż macierze";
            rbCompareMatrices.Content = "Porównaj macierze";
            tbAddToList.Text = "Dodać macierz wynikową do listy?";
            rbYes.Content = "Tak";
            rbNo.Content = "Nie";
            rbOverwrite.Content = "Nadpisz";
            bCalcOperation.Content = "Oblicz";
            tbOResult.Text = "Wynik";

            tbSLanguage.Text = "Język:";
            bSaveMatrices.Content = "Zapisz macierze";
            bOpenMatrices.Content = "Wczytaj macierze";
            bExit.Content = "Wyjście";
            wantSave = "Chcesz zapisać swoją listę?";
            tbVersion.Text = "Wersja:";
            tbAuthor.Text = "Autor:";

            (cbChangeLanguage.Items[0] as ComboBoxItem).Content = "Angielski";
            (cbChangeLanguage.Items[1] as ComboBoxItem).Content = "Polski";
        }

    }
}