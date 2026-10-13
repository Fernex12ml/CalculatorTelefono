using System;
using System.Data;
using System.Globalization;

namespace CalculatorTelefono
{
    public partial class MainPage : ContentPage
    {
        private string currentExpression = "0";
        private bool isResultShown = false;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnNumberClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string pressed = button.Text;

            if (isResultShown)
            {
                currentExpression = pressed;
                isResultShown = false;
            }
            else
            {
                if (currentExpression == "0")
                {
                    currentExpression = pressed;
                }
                else
                {
                    currentExpression += pressed;
                }
            }
            UpdateDisplay();
        }

        private void OnDecimalClicked(object sender, EventArgs e)
        {
            if (isResultShown)
            {
                currentExpression = "0.";
                isResultShown = false;
                UpdateDisplay();
                return;
            }

            string lastNumber = GetLastNumberSegment();
            if (!lastNumber.Contains("."))
            {
                currentExpression += ".";
                UpdateDisplay();
            }
        }

        private void OnOperatorClicked(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            string op = button.Text;

            if (isResultShown)
            {
                isResultShown = false;
            }

            if (currentExpression.Length > 0)
            {
                char lastChar = currentExpression[currentExpression.Length - 1];
                if ("+−×÷%".Contains(lastChar))
                {
                    currentExpression = currentExpression.Substring(0, currentExpression.Length - 1) + op;
                }
                else
                {
                    currentExpression += op;
                }
            }
            UpdateDisplay();
        }

        private void OnClearClicked(object sender, EventArgs e)
        {
            currentExpression = "0";
            isResultShown = false;
            UpdateDisplay();
        }

        private void OnBackspaceClicked(object sender, EventArgs e)
        {
            if (isResultShown)
            {
                currentExpression = "0";
                isResultShown = false;
            }
            else if (currentExpression.Length > 1)
            {
                currentExpression = currentExpression.Substring(0, currentExpression.Length - 1);
            }
            else
            {
                currentExpression = "0";
            }
            UpdateDisplay();
        }

        private void OnExtraClicked(object sender, EventArgs e)
        {
            currentExpression = "0";
            UpdateDisplay();
        }

        private async void OnEqualsClicked(object sender, EventArgs e)
        {
            try
            {
                string evalString = currentExpression
                    .Replace("×", "*")
                    .Replace("÷", "/")
                    .Replace("−", "-");

                if (ContainsDivisionByZero(evalString))
                {
                    await DisplayAlertAsync("Error", "No se puede dividir entre cero.", "OK");
                    currentExpression = "0";
                    UpdateDisplay();
                    return;
                }

                DataTable table = new DataTable();
                var result = table.Compute(evalString, string.Empty);
                double finalResult = Convert.ToDouble(result, CultureInfo.InvariantCulture);

                currentExpression = finalResult.ToString(CultureInfo.InvariantCulture);
                isResultShown = true;
            }
            catch (Exception)
            {
                await DisplayAlertAsync("Error", "Expresión matemática inválida.", "OK");
                currentExpression = "0";
            }
            UpdateDisplay();
        }

        private bool ContainsDivisionByZero(string expr)
        {
            string[] parts = expr.Split('/');
            for (int i = 1; i < parts.Length; i++)
            {
                string denominator = parts[i].TrimStart();
                if (denominator.StartsWith("0") && (denominator.Length == 1 || !char.IsDigit(denominator[1])))
                {
                    return true;
                }
            }
            return false;
        }

        private string GetLastNumberSegment()
        {
            int lastOpIndex = -1;
            char[] operators = { '+', '-', '*', '/', '%' };
            foreach (var op in operators)
            {
                int idx = currentExpression.LastIndexOf(op);
                if (idx > lastOpIndex) lastOpIndex = idx;
            }

            if (lastOpIndex == -1) return currentExpression;
            return currentExpression.Substring(lastOpIndex + 1);
        }

        private void UpdateDisplay()
        {
            lblDisplay.Text = currentExpression;
        }
    }
}