using System.Text;

public static class PersianText
{
    public static string Fix(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        
        // جایگزینی حروف با فرم‌های چسبیده
        input = input.Replace("ي", "\uFEF3"); // ی عربی
        input = input.Replace("ك", "\uFED3"); // ک عربی
        input = input.Replace("ی", "\uFEF3"); // ی فارسی
        input = input.Replace("ک", "\uFED3"); // ک فارسی
        
        return input;
    }
}
