
using UnityEngine;

public enum AlphabetEnum
{
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z
}

[CreateAssetMenu(menuName = "Audio/Alphabet Sounds Library")]
public class AlphabetSoundsLibrary : AudioLibrary<AlphabetEnum> { }
