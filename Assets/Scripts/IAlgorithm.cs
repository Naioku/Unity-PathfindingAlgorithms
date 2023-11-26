using UnityEngine;

public interface IAlgorithm
{
    void Initialize(Maze maze, Vector2Int startCoords, Vector2Int destinationCoords);
    void Play();
    void Pause();
    void Step();
    void Refresh();
    void Stop();
}