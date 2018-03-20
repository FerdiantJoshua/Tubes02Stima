using System;
using System.Collections.Generic;
/*using System.Linq;
using System.Text;
using System.Threading.Tasks;*/
using System.Windows;
using System.Windows.Forms;
/*using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;*/
using TopologicalSorting;
using Microsoft.Glee.Drawing;

namespace Tubes02Stima
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        List<DAG> listOfDAG = new List<DAG>();
        List<MataKuliah> dataMatkul = new List<MataKuliah>();

        List<DAG> dfsResult = new List<DAG>();
        List<DAG> bfsResult = new List<DAG>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnOperate_Click(object sender, RoutedEventArgs e)
        {
            //Mengambil nama file eksternal dari txtBoxFileName
            string fileName = txtBoxFileName.Text;
            //Mengosongkan textBox hasil BFS dan DFS
            txtDFSResult.Clear();
            txtBFSResult.Clear();

            //==========================INISIALISASI OBJEK2 DAN VARIABEL2 UNTUK VISUALISASI==========================
            Form form = new Form();

            form.ClientSize = new System.Drawing.Size(1000, 768);

            //Buat objek-viewer
            Microsoft.Glee.GraphViewerGdi.GViewer viewer = new Microsoft.Glee.GraphViewerGdi.GViewer();

            //Buat objek-graph
            Microsoft.Glee.Drawing.Graph graph = new Microsoft.Glee.Drawing.Graph("graph");
            
            string nodeName, childNodeName;

            //==========================BACA INPUT DARI FILE EKSTERNAL==========================
            TopologicalSorting.Main.MakeDAGFromFile(ref listOfDAG, ref dataMatkul, fileName);
            /*Console.WriteLine("listOfDAG :");
            TopologicalSorting.Main.PrintListOfDAG(listOfDAG);
            Console.WriteLine();*/


            //==========================DFS==========================
            TopologicalSorting.Main.DFS(listOfDAG, ref dfsResult);
                //Console.WriteLine("\nDFS Result : ");
                //TopologicalSorting.Main.PrintListOfDAG(dfsResult);
            //Cetak hasil DFS ke textbox
            foreach (var dag in dfsResult)
            {
                txtDFSResult.Text += dag.GetEnumerator() + "/" + dag.GetDenumerator() + ". " + dag.GetNode() + "\n";
            }

            //Buat graph-nya
            graph.AddNode("StartDFS");
            graph.FindNode("StartDFS").Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGray;
            foreach (var dag in dfsResult)
            {
                nodeName = dag.GetEnumerator() + "/" + dag.GetDenumerator() + ". " + dag.GetNode();
                //Warnai semua matakuliah(simpul) pada diagram BFS dengan warna orchid
                graph.AddNode(nodeName).Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orchid;
                foreach (var child in dag.GetListOfChildren())
                {
                    childNodeName = child.GetEnumerator() + "/" + child.GetDenumerator() + ". " + child.GetNode();

                    graph.AddEdge(nodeName, childNodeName);
                }
                if (dag.GetNParents() == 0)
                {
                    graph.AddEdge("StartDFS", nodeName);
                }
            }

            //==========================BFS==========================
            TopologicalSorting.Main.BFS(listOfDAG, ref bfsResult);
                //Console.WriteLine("\nBFS Result : ");
                //TopologicalSorting.Main.PrintListOfDAG(bfsResult);
            //Cetak hasil BFS ke textbox
            int sem = 1;
            foreach (var dag in bfsResult)
            {
                if(dag.GetSemester() == sem)
                {
                    txtBFSResult.Text += "Semester " + dag.GetSemester() + ":\n";
                    sem++;
                }
                txtBFSResult.Text += dag.GetEnumerator() + ". " + dag.GetNode() + "\n";
            }
            
            //Buat graph-nya
            graph.AddNode("StartBFS");
            graph.FindNode("StartBFS").Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGray;
            foreach (var dag in bfsResult)
            {
                nodeName = dag.GetEnumerator() + ". " + dag.GetNode();
                graph.AddNode(nodeName);
                Microsoft.Glee.Drawing.Node tNode;
                foreach (var child in dag.GetListOfChildren())
                {
                    childNodeName = child.GetEnumerator() + ". " + child.GetNode();

                    graph.AddEdge(nodeName, childNodeName);
                    tNode = graph.FindNode(childNodeName);
                    //Warnai matakuliah(simpul) dengan 1 warna per semester
                    switch (child.GetSemester() % 6)
                    {
                        case 0:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightSkyBlue;
                            break;
                        case 1:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Orange;
                            break;
                        case 2:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.PaleVioletRed;
                            break;
                        case 3:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.LightGreen;
                            break;
                        case 4:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.Yellow;
                            break;
                        case 5:
                            tNode.Attr.Fillcolor = Microsoft.Glee.Drawing.Color.MediumPurple;
                            break;
                    }
                }
                if (dag.GetNParents() == 0)
                {
                    graph.AddEdge("StartBFS", nodeName);
                }
            }

            //==========================VISUALISASI==========================
            //Hubungkan graph dengan viewernya
            viewer.Graph = graph;

            //Hubungkan viewer dengan formnya
            form.SuspendLayout();
            viewer.Dock = DockStyle.Fill;
            form.Controls.Add(viewer);
            form.ResumeLayout();

            //Tampilkan formnya
            form.ShowDialog();
        }
    }
}
