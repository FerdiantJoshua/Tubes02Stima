using System;
using System.Collections.Generic;
using System.Linq;
/*using System.Text;
using System.Threading.Tasks;*/
using System.IO;
using System.Text.RegularExpressions;

namespace TopologicalSorting
{
    struct MataKuliah {
        public string kodeKuliah;
        public string[] listOfPrasyarat;
    }

    class DAG
    {
        private string node; //menyimpan nama matakuliah
        private int enumerator, denumerator; //menyimpan nilai urutan pemrosesan simpul (denumerator hanya digunakan oleh DFS)
        private List<DAG> children; //menyimpan alamat objek matakuliah(simpul) di memory yang dapat diambil setelah matakuliah(simpul) ini diambil
        private List<DAG> parents; //menyimpan alamat objek matakuliah(simpul) di memory yang menjadi prasyarat dari matakuliah(simpul) ini 
        private int semester; //menyimpan "pada semester ke berapa matakuliah(simpul) ini dapat diambil" (hanya digunakan oleh BFS)
        private bool processed; //menyimpan nilai yang menentukan apakah matakuliah(simpul) sudah dikunjungi atau belum

        //Ctor
        public DAG(string val)
        {
            this.node = val;
            enumerator = 0;
            denumerator = 0;
            children = new List<DAG>();
            parents = new List<DAG>();
            semester = 0;
            processed = false;
        }

        //Getter & Setter
        public string GetNode()
        {
            return this.node;
        }
        public void SetNode(string nod)
        {
            this.node = nod;
        }
        public int GetEnumerator()
        {
            return this.enumerator;
        }
        public void SetEnumerator(int enu)
        {
            this.enumerator = enu;
        }
        public int GetDenumerator()
        {
            return this.denumerator;
        }
        public void SetDenumerator(int den)
        {
            this.denumerator = den;
        }
        public int GetSemester()
        {
            return this.semester;
        }
        public void SetSemester(int sem)
        {
            this.semester = sem;
        }
        public bool GetProcessed()
        {
            return this.processed;
        }
        public void SetProcessed(bool pro)
        {
            this.processed = pro;
        }

        //Main functions and methods
        public void AddChild(DAG T)
        {
            this.children.Add(T);
        }
        public DAG GetChild(int i)
        {
            if (i < this.children.Count)
            {
                return this.children[i];
            }
            else {
                return null;
            }
        }
        public List<DAG> GetListOfChildren()
        {
            return this.children;
        }
        public int GetNChildren()
        {
            return this.children.Count();
        }
        public int GetNbUnprocessedChild()
        {
            int result = 0;

            foreach (var child in this.children)
            {
                if (child.GetProcessed() == false) result++;
            }

            return result;
        }

        public void AddParent(DAG T)
        {
            this.parents.Add(T);
        }
        public DAG GetParent(int i)
        {
            if (i < this.parents.Count)
            {
                return this.parents[i];
            }
            else
            {
                return null;
            }
        }
        public List<DAG> GetListOfParents()
        {
            return this.parents;
        }
        public int GetNParents()
        {
            return this.parents.Count();
        }
        public int GetNbUnprocessedParent()
        {
            int result = 0;

            foreach (var parent in this.parents)
            {
                if (parent.GetProcessed() == false) result++;
            }

            return result;
        }
    }

    class Main
    {
        public static void MakeDAGFromFile(ref List<DAG> listOfDAG, ref List<MataKuliah> dataMatkul, string fileName)
            //Prosedur untuk membaca file eksternal, kemudian menyimpannya ke dalam struktur data DAG
            //I.S Menerima himpunan simpul awal, struct dataMatakuliah yang masih kosong dan nama file eksternal
            //F.S Himpunan simpul awal dan struct dataMataKuliah sudah terisi sesuai data yang terdapat dalam file eksternal

        {
            //Inisialisasi listOfDAG dan dataMatkul
            listOfDAG = new List<DAG>();
            dataMatkul = new List<MataKuliah>();

            //Baca dari file eksternal, simpen di list of string
            List<string> list_of_input = new List<string>();
            using (var reader = new StreamReader(fileName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    list_of_input.Add(line);
                }
            }

            foreach (var line in list_of_input)
            {
                //Pisahkan tiap input berdasarkan spasi dan koma
                //Console.WriteLine($"Regex result : {Regex.Replace(line, @"[ ]*[.]*", "")}");
                string[] lines = Regex.Replace(line, @"[ ]*[.]*", "").Split(',', '.');
                //Simpan dalam list of matakuliah
                MataKuliah temp;
                temp.kodeKuliah = lines[0];
                temp.listOfPrasyarat = new string[lines.Length - 1];
                for (int i = 1; i < lines.Length; i++)
                {
                    temp.listOfPrasyarat[i - 1] = lines[i];
                }
                dataMatkul.Add(temp);

                //Buat pohonnya, hanya assign nilai node-nya saja
                DAG dag = new DAG(temp.kodeKuliah);
                listOfDAG.Add(dag);


                /*Console.WriteLine($"Kuliah : {temp.kodeKuliah}");
                Console.WriteLine("Prasyarat :");
                foreach (var prasyarat in temp.listOfPrasyarat)
                {
                    Console.WriteLine(prasyarat);
                }*/
            }

            int idx;
            //Assign orangtuanya
            idx = 0;
            foreach (var matkul in dataMatkul)
            {
                foreach (var prasyarat in matkul.listOfPrasyarat)
                {
                    foreach (var dag in listOfDAG)
                    {
                        //Cari pohon yang namanya = prasyarat, masukkan ke dalam listOfParents
                        if (dag.GetNode() == prasyarat)
                        {
                            listOfDAG[idx].AddParent(dag);
                        }
                    }
                }
                idx++;
            }

            //Assign anak2nya
            idx = 0;
            foreach (var matkul in dataMatkul)
            {
                foreach (var prasyarat in matkul.listOfPrasyarat)
                {
                    foreach (var dag in listOfDAG)
                    {
                        //Cari pohon yang namanya = prasyarat, masukkan ke dalam listOfChildren pohon tsb
                        if (dag.GetNode() == prasyarat)
                        {
                            dag.AddChild(listOfDAG[idx]);
                        }
                    }
                }
                idx++;
            }

            //listOfDAG[0].SetProcessed(true);
            //PrintListOfDAG(listOfDAG);
        }

        public static void PrintListOfDAG(List<DAG> listOfDAG)
            //Fungsi untuk debugging, menampilkan setiap matakuliah(simpul), orangtuanya, anaknya dan status masing2 simpul
        {
            foreach (var dag in listOfDAG)
            {
                Console.WriteLine($"Pohon {dag.GetNode()}, processed : {dag.GetProcessed()}, Enum/Denum : {dag.GetEnumerator()}/{dag.GetDenumerator()}");
                Console.WriteLine("Orangtua :");
                foreach (var parent in dag.GetListOfParents())
                {
                    Console.WriteLine($"\t{parent.GetNode()}, processed : {parent.GetProcessed()}");
                }
                Console.WriteLine("Anak:");
                foreach (var child in dag.GetListOfChildren())
                {
                    Console.WriteLine($"\t{child.GetNode()}, processed : {child.GetProcessed()}");
                }
            }
        }

        public static void Initialization(List<DAG> listOfDAG, ref List<DAG> lodResult)
            //Prosedur inisialisasi yang digunakan oleh prosedur DFS() dan BFS()
        {
            lodResult = new List<DAG>();
            foreach (var dag in listOfDAG)
            {
                dag.SetProcessed(false);
                dag.SetSemester(0);
                dag.SetEnumerator(0);
                dag.SetDenumerator(0);
            }
        }

        public static void DFS(List<DAG> listOfDAG, ref List<DAG> lodResult)
            //Prosedur yang mengatur algoritma DFS, memanggail fungsi DFSProcess()
            //Prosedur ini TIDAK mencatat matakuliah yang dapat diambil persemester
            //I.S. Menerima himpunan simpul awal
            //F.S. Mengembalikan himpunan solusi simpul setelah diurutkan secafa DFS
        {
            //Inisialisasi
            int counter = 1;
            DAG startingNode = new DAG("");
            List<DAG> queue = new List<DAG>();
            List<DAG> temp = new List<DAG>();
            Initialization(listOfDAG, ref lodResult);

            //Bangkitkan semua matakuliah(simpul) yang tidak memiliki prasyarat
            foreach (var dag in listOfDAG)
            {
                //Mulai proses DFS/BFS dari matakuliah yang tidak memiliki prasyarat (simpul masuk)
                if (dag.GetNbUnprocessedParent() == 0 && !dag.GetProcessed())
                {
                    queue.Add(dag);
                }
            }
            
            //Proses setiap matakuliah(simpul) yang tidak memiliki prasyarat
            for (int i = 0; i < queue.Count(); i++)
            {
                startingNode = queue[i];
                temp = DFSProcess(startingNode, ref counter);
                //Append hasil DFS ke bagian depan himpunan solusi (lodResult)
                int k = 0;
                for (k = temp.Count() - 1; k >= 0; k--)
                {
                    lodResult.Insert(0, temp[k]);
                }
            }
        }

        private static List<DAG> DFSProcess(DAG startingNode, ref int counter)
            //Fungsi rekursif untuk memproses secara DFS
            //I.S. Menerima simpul mulai dan counter (yang akan digunakan sebagai enumerator/denumerator)
            //F.S. Mengembalikan himpunan solusi simpul setelah diurutkan secara DFS
        {
            List<DAG> lodResult = new List<DAG>();
            List<DAG> temp= new List<DAG>();
            int i = 0;

            //Pencatatan nilai enumerator
            startingNode.SetEnumerator(counter);
            counter++;
            //Jika simpul masih memiliki anak yang belum dikunjungi, cari anak yang belum dikunjungi tersebut
            while (startingNode.GetNbUnprocessedChild() != 0)
            {
                //Pencarian anak yang belum dikunjungi
                while(i < startingNode.GetNChildren() && startingNode.GetChild(i).GetProcessed())
                {
                    i++;
                }

                //Jika ketemu
                if (i < startingNode.GetNChildren())
                {
                    //Periksa dan proses anak tsb
                    Console.WriteLine($"Checking node : {startingNode.GetChild(i).GetNode()}.");
                    temp = DFSProcess(startingNode.GetChild(i), ref counter);
                    //Append hasil DFS ke bagian depan himpunan solusi (lodResult)
                    int k = 0;
                    for (k = temp.Count() - 1; k >= 0; k--)
                    {
                        lodResult.Insert(0, temp[k]);
                    }
                }
            }

            //SETELAH SEMUA ANAK DIPROSES :
            //Pencatatan nilai denumerator
            startingNode.SetProcessed(true);
            startingNode.SetDenumerator(counter);
            counter++;

            //Simpan nilai simpul ke dalam himpunan solusi (lodResult)
            lodResult.Insert(0, startingNode);
            Console.WriteLine($"Node : {startingNode.GetNode()} is added.");

            Console.WriteLine("Current lodResult : ");
            foreach (var dag in lodResult)
            {
                Console.Write($"{dag.GetNode()} ");
            }
            Console.WriteLine();

            //Return value
            return lodResult;
        }

        public static void BFS(List<DAG> listOfDAG, ref List<DAG> lodResult)
            //Prosedur yang melakukan algoritma BFS
            //Prosedur ini mencatat matakuliah yang dapat diambil per semester
            //I.S. Menerima himpunan simpul awal
            //F.S. Mengembalikan himpunan solusi simpul setelah diurutkan secara BFS
        {
            //Inisialisasi
            int counter = 1;
            int semesterNumber = 1;
            DAG startingNode = new DAG("");
            List<DAG> queue = new List<DAG>();
            List<DAG> temp = new List<DAG>();
            Initialization(listOfDAG, ref lodResult);

            //Proses ini diulangi sebanyak jumlah matakuliah(simpul)
            for (int i = 0; i < listOfDAG.Count(); i++)
            {
                //Bangkitkan semua matakuliah(simpul) yang sudah tidak memiliki prasyarat (simpul masuk)
                queue.Clear();
                foreach (var dag in listOfDAG)
                {
                    if (dag.GetNbUnprocessedParent() == 0 && !dag.GetProcessed())
                    {
                        queue.Add(dag);
                    }
                }

                //Proses setiap matakuliah(simpul) yang tidak memiliki prasyarat
                for (int j = 0; j < queue.Count(); j++)
                {
                    startingNode = queue[j];
                    startingNode.SetEnumerator(counter);
                    counter++;

                    startingNode.SetSemester(semesterNumber);

                    //Nilai denumerator tidak dicatat
                    //Ubah nilai boolean processed menjadi true
                    startingNode.SetProcessed(true);

                    //Simpan nilai simpul ke dalam himpunan solusi (lodResult)
                    lodResult.Add(startingNode);
                    Console.WriteLine($"Node : {startingNode.GetNode()} is added.");

                    Console.WriteLine("Current lodResult : ");
                    foreach (var dag2 in lodResult)
                    {
                        Console.Write($"{dag2.GetNode()} ");
                    }
                    Console.WriteLine();
                }

                semesterNumber++;
            }
        }
    }
}
