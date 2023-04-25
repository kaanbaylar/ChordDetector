using System;
using System.IO;
using NAudio.Wave;

// Kodun �al��mas� i�in "NAudio" paketinin kurulu olmas� gerektir.
// FFT (H�zl� Fourier D�n���m�) kullanarak sesin frekans bile�enlerini hesaplayabilir.
// Kodda, `Complex[]` t�r�ndeki `fftBuffer` dizisi, FFT i�leminden sonra elde edilen frekans bile�enlerini i�erir.

namespace Mp3ChordFinder
{
    class Program
    {
        static void Main(string[] args)
        {
            // Kullan�c�dan mp3 dosyas�n�n yolunu al
            Console.Write("Enter the path to the mp3 file: ");
            string filePath = Console.ReadLine();

            // Dosya var m� diye kontrol et
            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist!");
                return;
            }

            // Ses �zelliklerini al
		WaveFormat format = reader.WaveFormat;
		int channels = format.Channels;
		int sampleRate = format.SampleRate;

	    // Ses verilerini al
		byte[] buffer = new byte[reader.Length];
		reader.Read(buffer, 0, (int)reader.Length);

	    // Mp3 dosyas�n� a� ve okuma ba�lat
            using (Mp3FileReader reader = new Mp3FileReader(filePath))
            {
                // Sample rate ve kanal say�s� bilgilerini al
                int sampleRate = reader.WaveFormat.SampleRate;
                int channels = reader.WaveFormat.Channels;

                // FFT boyutu belirle
                int fftSize = 4096;
	
		// FFT i�in yeni bir buffer olu�tur
		Complex[] fftBuffer = new Complex[fftSize];

                // Akorlar ve frekans bile�enleri
                string[] chords = { "A", "Am", "B", "C", "D", "Dm", "E", "Em", "F", "G" };
                double[][] chordFrequencies = new double[][] {
                    new double[] { 82.4, 110.0, 146.8 },  // A
                    new double[] { 82.4, 110.0, 131.0 },  // Am
                    new double[] { 98.0, 123.5, 164.8 },  // B
                    new double[] { 65.4, 98.0, 130.8 },   // C
                    new double[] { 73.4, 110.0, 146.8 },  // D
                    new double[] { 73.4, 110.0, 131.0 },  // Dm
                    new double[] { 82.4, 123.5, 164.8 },  // E
                    new double[] { 82.4, 123.5, 147.0 },  // Em
                    new double[] { 87.3, 130.8, 174.6 },  // F
                    new double[] { 98.0, 146.8, 196.0 }   // G
                };

		// Her bir frekans�n ne kadar yak�n olmas� gerekti�i
		double frequencyTolerance = 5.0;

                // Okuma ba�lat
                byte[] buffer = new byte[fftSize * channels * 2];
                int bytesRead = reader.Read(buffer, 0, buffer.Length);

                while (bytesRead > 0)
                {
                    // �rnekleme verilerini �ift olarak d�n��t�r
                    double[] sampleBuffer = new double[fftSize];
                    for (int i = 0; i < bytesRead / 2; i++)
                    {
                        short sample = BitConverter.ToInt16(buffer, i * 2);
                        sampleBuffer[i] = (double)sample / 32768.0;
                    }

                    // FFT hesapla
                    double[] fftBuffer = new double[fftSize];
                    FourierTransform.FFT(sampleBuffer, fftBuffer, false);

		    // Her bir akorun frekanslar�n� hesapla
			double[][] chordFrequencies = new double[chords.Length][];
			for (int i = 0; i < chords.Length; i++)
			{
   	 			chordFrequencies[i] = new double[3];
    				for (int j = 0; j < 3; j++)
   				 {
       					 chordFrequencies[i][j] = baseFrequencies[i] * (Math.Pow(2.0, j / 3.0));
   				 }
			}

		    // FFT'den elde edilen frekans bile�enlerini kullanarak hangi akorun �al�nd���n� belirle
 		    int chordIndex = -1;
	    	    double minError = double.MaxValue;
		    for (int i = 0; i < chords.Length; i++)
		    {
    			double error = 0.0;
    			for (int j = 0; j < 3; j++)
    			{
        			double closestFrequency = 0.0;
        			double minDistance = double.MaxValue;
        			for (int k = 0; k < fftSize / 2; k++)
        			{
          			  double frequency = (double)k * sampleRate / (double)fftSize;
           			  double distance = Math.Abs(frequency - chordFrequencies[i][j]);
           			  if (distance < minDistance)
            			  {
            			    minDistance = distance;
              			    closestFrequency = frequency;
           			  }
       		        }
     		        error += minDistance;
   		 }
   	         if (error < minError)
 	         {
     		   chordIndex = i;
     		   minError = error;
    		 }
	    }

		// Sonucu konsola yazd�r
		Console.WriteLine("Chord: " + chords[chordIndex]);


                // �al�nan akoru yazd�r
                Console.WriteLine("Chord: " + chords[chordIndex]);

                // Bir sonraki �rnekleme verilerini oku
                bytesRead = reader.Read(buffer, 0, buffer.Length);
            }
        }

        // Program�n sonuna geldi�imizi belirt
        Console.WriteLine("Done!");
        Console.ReadKey();
    }
}
}