using NHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Paycore_Net_Bootcamp_Hafta_4.KMeanAlgorithm
{
    public class KMeanAlgorithmClass
    {
        /// <summary>
        /// K-Means algoritmasının çalıştığı fonksiyon
        /// Basta rastgele kümelere atan versiyonuyla çalışmaktadır algoritma
        /// </summary>
        /// <param name="data"></param>
        /// <param name="clusterCount"></param>
        /// <returns>Geriye container ların hangi kümeye ait oldugunu index numaraları olarak döndürür. 
        /// Her index numarası bir kümeye karşılık gelir
        /// </returns>
        public static IList<int> KMeans(double[][] data, int clusterCount)
        {
            var random = new Random();
            // kümeye olan uzaklıkta kullanmak icin küme ve degerlerini tutan Tuple data tipinde liste
            var resultCluster = new List<(int assignedCluster, double[] Values)>(); 
            bool isDifferentCluster = false;

            // Kümelerin indexlerini tutan liste bunu kullanarak her bir kümede elemanın olup olmadıgını kontrol edeceğiz
            var n = Enumerable.Range(0, clusterCount).ToList();
            do
            {
                // Her satırı rastgele bir kümeye ata
                resultCluster = Enumerable
                                   .Range(0, data.Length)
                                   .Select(index => (assignedCluster: random.Next(0, clusterCount), Values: data[index]))
                                   .ToList();

                n = Enumerable.Range(0, clusterCount).ToList(); // cluster indexleri 0,1,2,3,4,5,6,7 ...
                for (int i = 0; i < resultCluster.Count; i++)
                {
                    // eger listede eleman kaldıysa ve varsa atanan cluster numarası atanmıs cluster numarasını kaldır
                    if (n.Count > 0) n.Remove(resultCluster[i].assignedCluster);
                }
                isDifferentCluster = n.Count == 0;
                
            } while (!isDifferentCluster);

            var dimensionNumber = data[0].Length;

            // Kümelere olan uzaklıkların sürekli değişmesi durumunda sonsuz döngüye girer bunu engellemek için limit belirledim
            var limit = 10000;
            // Güncellenmediği zaman artık döngüden çıkmak için kullanmak için
            var isUpdated = true;
            while (--limit > 0)
            {
                // kümelerin merkez noktalarını hesapla
                var centerPoints = Enumerable.Range(0, clusterCount)
                                                .AsParallel()
                                                .Select(clusterNumber =>
                                                (
                                                cluster: clusterNumber,
                                                centerPoint: Enumerable.Range(0, dimensionNumber)
                                                                       .Select(axis => 
                                                                       resultCluster.Where(s => s.assignedCluster == clusterNumber).Average(s => s.Values[axis]))
                                                                                    .ToArray())
                                                        ).ToArray();
                // Sonuç kümesini merkeze en yakın ile güncelle
                isUpdated = false;
                //for (int i = 0; i < resultCluster.Count; i++)
                Parallel.For(0, resultCluster.Count, i =>
                {
                    var row = resultCluster[i];
                    var oldassignedCluster = row.assignedCluster;

                    var newassignedCluster = centerPoints.Select(n => (ClusterNumber: n.cluster,
                                                                    Distance: CalculateDistance(row.Values, n.centerPoint)))
                                         .OrderBy(x => x.Distance)
                                         .First()
                                         .ClusterNumber;

                    if (newassignedCluster != oldassignedCluster)
                    {
                        resultCluster[i] = (assignedCluster: newassignedCluster, Values: row.Values);
                        isUpdated = true;
                    }
                });

                if (!isUpdated)
                {
                    break;
                }
            } // while

            return resultCluster.Select(k => k.assignedCluster).ToArray();
        }

        //Containerların küme merkezine olan uzaklıkları hesaplamak icin kullanılır
        static double CalculateDistance(double[] firstPoint, double[] secondPoint)
        {
            var squareDistance = firstPoint
                                    .Zip(secondPoint,
                                        (n1, n2) => Math.Pow(n1 - n2, 2)).Sum();
            return Math.Sqrt(squareDistance);
        }
    }
}
