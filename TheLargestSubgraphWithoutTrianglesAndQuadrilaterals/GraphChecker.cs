using System;
using System.Collections.Generic;

namespace V3
{
    public class GraphChecker
    {
        // Функция, которая проверяет, есть ли цикл на трех или четырех вершинах в графе
        public static bool HasCycle(Dictionary<int, List<int>> graph)
        {
            // Проходим по всем вершинам графа
            foreach (var node in graph.Keys)
            {
                // Создаем множество для хранения посещенных вершин
                HashSet<int> visited = new HashSet<int>();
                // Добавляем текущую вершину в множество
                visited.Add(node);
                // Вызываем рекурсивную функцию для поиска цикла из текущей вершины
                if (HasCycleHelper(graph, node, node, visited))
                {
                    // Если нашли цикл, возвращаем true
                    return true;
                }
            }
            // Если не нашли цикл, возвращаем false
            return false;
        }

        // Рекурсивная функция, которая проверяет, есть ли цикл из текущей вершины
        public static bool HasCycleHelper(Dictionary<int, List<int>> graph, int start, int current, HashSet<int> visited)
        {
            // Проходим по всем смежным вершинам текущей вершины
            foreach (var neighbor in graph[current])
            {
                // Если смежная вершина равна стартовой и длина цикла равна 3 или 4, то возвращаем true
                if (neighbor == start && (visited.Count == 3 || visited.Count == 4))
                {
                    return true;
                }
                // Если смежная вершина не посещена и длина цикла меньше 4, то добавляем ее в множество и продолжаем поиск из нее
                if (!visited.Contains(neighbor) && visited.Count < 4)
                {
                    visited.Add(neighbor);
                    if (HasCycleHelper(graph, start, neighbor, visited))
                    {
                        return true;
                    }
                    // Возвращаем смежную вершину из множества, если не нашли цикл из нее
                    visited.Remove(neighbor);
                }
            }
            // Если не нашли цикл из текущей вершины, возвращаем false
            return false;
        }
    }
}