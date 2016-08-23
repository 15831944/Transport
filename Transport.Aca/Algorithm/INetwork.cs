using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Transport.Aca.Model;

namespace Transport.Aca.Algorithm
{
    public interface INetwork
    {
        // матрица смежности
        double[,] AdjacencyMatrix { get; set; }
        // матрица корреспонденций
        double[,] DirectTravelersMatrix { get; set; }

        Node[] Nodes { get; }
        // Размерность матрицы смежности
        int Size { get; }

        // событие обновления сети
        event EventHandler AdjacencyMatrixUpdated;
        event EventHandler DirectTravelrsMatrixUpdated;
        event EventHandler NodesUpdated;
    }
}
