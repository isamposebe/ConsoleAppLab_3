using System;
using System.Diagnostics;

class Program
{
    /// <summary>
    /// Функция
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    static double f(double x)
    {
        // здесь нужно задать подынтегральную функцию
        return Math.Sin(x);
    }


    /// <summary>
    ///     Вичисления интеграла
    /// </summary>
    /// <param name="a">Начало отрезка</param>
    /// <param name="b">Конец отрезка</param>
    /// <param name="N">Кол-во отрезков</param>
    /// <returns>Решение интеграла</returns>
    static double Integral(double a, double b, int N)
    {
        double h = (b - a) / N;
        double sum = 0;

        for (int i = 0; i < N; i++)
        {
            double x = a + h * (i + 0.5);
            sum += f(x) * h;
        }

        return sum;
    }

    /// <summary>
    ///      реализует параллельное вычисление интеграла
    /// </summary>
    /// <param name="a">Начало отрезка</param>
    /// <param name="b">Конец отрезка</param>
    /// <param name="r">разбивает отрезок на r равных отрезков</param>
    /// <returns></returns>
    static double ParallelIntegral(double a, double b, int r)
    {
        double h = (b - a) / r;
        double sum = 0;

        Parallel.For(0, r, i =>
        {
            double localA = a + h * i;
            double localB = localA + h;
            double localSum = Integral(localA, localB, 1000);
            lock (typeof(Program))
            {
                sum += localSum;
            }
        });

        return sum;
    }
    static double MonteCarloMethod(double a, double b, int n, Func<double, double> f)
    {
        Random rand = new Random();
        int count = 0;
        for (int i = 0; i < n; i++)
        {
            double x = a + rand.NextDouble() * (b - a);
            double y = rand.NextDouble();

            if (y <= f(x))
            {
                count++;
            }
        }

        return (double)count / n * (b - a);
    }

    public static double PyramidIntegration(Func<double, double> f, double a, double b, int n)
    {
        double h = (b - a) / n;
        double sum = 0.0;
        double x = a + 0.5 * h;

        for (int i = 0; i < n; i++)
        {
            sum += f(x);
            x += h;
        }

        double result = sum * h;

        for (int k = 1; k < n; k *= 2)
        {
            h /= 2.0;
            sum = 0.0;
            x = a + 0.5 * h;

            for (int i = 0; i < n / (2 * k); i++)
            {
                sum += f(x);
                x += h;
            }

            result = 0.5 * (result + sum * h);
        }

        return result;
    }




    static void Main(string[] args)
    {
        int time = 0;
        double a = 0;
        double b = Math.PI;
        int r = 4;
        int n = 1000000; // число случайных точек
        ///Расчет времени
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        double integral = ParallelIntegral(a, b, r);
        stopwatch.Stop();
        time = int.Parse(stopwatch.ElapsedMilliseconds.ToString());
        Console.WriteLine($"Паралейный Integral = {integral}, Time elapsed = {time} ms");
        stopwatch.Start();

        double monteCarloMethod = MonteCarloMethod(a, b, n, f);
        stopwatch.Stop();

        time = int.Parse(stopwatch.ElapsedMilliseconds.ToString()) - time;
        Console.WriteLine($"метод Монте-Карло Integral = {monteCarloMethod}, Time elapsed = {time} ms");

        stopwatch.Start();

        double integ = Integral(a, b, n);
        stopwatch.Stop();

        time = int.Parse(stopwatch.ElapsedMilliseconds.ToString()) - time;
        Console.WriteLine($"Обычный Integral = {integral}, Time elapsed = {time} ms");
        stopwatch.Start();

        double pyramidIntegration = PyramidIntegration(f, a, b, n);
        stopwatch.Stop();

        time = int.Parse(stopwatch.ElapsedMilliseconds.ToString()) - time;
        Console.WriteLine($"реализации пирамидального метода Integral = {integral}, Time elapsed = {time} ms");


    }
}
