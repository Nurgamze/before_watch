using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Training.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
        NewRecord:
            Console.WriteLine("Öğrenci Ortalama Ve Harf Notu Hesaplama Programı");
            Console.WriteLine("_________________________________________________");

            //Yeni kayit kısmı için atama oluşturduk
            Console.WriteLine("\nÖğrenci Adını Girin: ");
            //değişken isimlerinde camel case kullanıyoruz
            string studentName = Console.ReadLine();

            Console.WriteLine("Öğrenci Numarasini Girin: ");
            string studentNumber = Console.ReadLine();

            Console.WriteLine("Öğrenci Vize Notu Girin: ");
            int midExamScore = int.Parse(Console.ReadLine());

            Console.WriteLine("Öğrenci Final Notu Girin: ");
            int studentFinalScore = int.Parse(Console.ReadLine());

            //CTRL + K + C yorum satırına alıyor

            //string name1 = "Gamzenur Demir";
            //char grade = 'A';
            //byte score = 95;
            //long TC = 54655188188782;
            //double average = 50.5;

            //CTRL + K + U yorum satırını kaldırıyor


            // CTRL + K + D düzeltir düzeni

            //Console.WriteLine(name1 + " " + grade + " " + score + " " + TC + " " + average);

            float averageScore = ((midExamScore * 30 + studentFinalScore * 70) / 100f);

            Console.WriteLine("Öğrencinin Ortalaması: " + averageScore);

            string grade;
            string passingStatus;

            if (averageScore >= 75)
            {
                grade = "AA";
                passingStatus = "Başarılı!";
            }
            else if (averageScore < 75 && averageScore >= 50)
            {
                grade = "CC";
                passingStatus = "Geçti!";
            }
            else
            {
                grade = "DD";
                passingStatus = "Başarısız!";
            }

            Console.WriteLine("\nHarf Notu: " + grade + "\nGeçme Durumu: " + passingStatus);


        İslemler:
            Console.WriteLine("\nÇıkmak için Exit (ESC) tuşuna basın! \nKaydetmek için 'S' tuşuna basın! \nİptal edip başa dönmek için 'C' tuşuna basın!");

            //kullanıcıdan veri alıyoruz ve atıyoruz
            var pressedKey = Console.ReadKey();
            if (pressedKey.Key == ConsoleKey.Escape)
            {
                Console.Clear();
                Console.WriteLine("Bye!");
            }
            //metin dosyasına kaydetmek istiyorsa
            else if (pressedKey.Key == ConsoleKey.S)
            {
                //bir diziye atıyoruz
                string[] informations = { "#######", studentName, studentNumber, averageScore.ToString(), grade, passingStatus };

                //gamzenur_demir\gamzenur_demir_54628088506_proje\Training.ConsoleApp\bin\Debug klasörünün içine kaydedecektir
                //yol belirtirken başına @ koyulur
                System.IO.File.AppendAllLines(@"students_scores.txt", informations);
                Console.WriteLine("\nKaydedildi. Yeni bir kayıt eklemek ister misiniz? Evet için Y 'ye Hayır için N'ye basınız.");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                {
                    goto NewRecord;
                }
                //yeni kayıt eklemek istemiyorsa
                if (Console.ReadKey().Key == ConsoleKey.N)
                {
                    Console.Clear();
                    Console.WriteLine("Bye!");
                }
            }
            else if (pressedKey.Key == ConsoleKey.C)
            {
                Console.Clear();
                goto NewRecord;
            }
            else
            {
                goto İslemler;
            }

            //programı durdurmak için yazıyoruz
            Console.ReadLine();
        }
    }
}
