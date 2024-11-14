using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Matrix3x3 : MonoBehaviour
{
    public float[,] values = new float[3, 3];

    //Constructors
    public Matrix3x3()
    {
        values = new float[3,3];
    }

    public Matrix3x3(float[,] _values)
    {
        if (checkIfValid(_values))
        {
            throw new System.ArgumentException("Initial values must be a 3x3 array.");
        }

        this.values = _values;
    }

    public static Matrix3x3 Identity()
    {
        Matrix3x3 idm = new Matrix3x3();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i == j)
                {
                    idm.values[i, j] = 1;
                }
                else
                {
                    idm.values[i, j] = 0;
                }
            }
        }

        return idm;
    }

    //Setter/Getter
    public float this[int row, int col]
    {
        get
        {
            if (row < 0 || row >= 3 || col < 0 || col >= 3)
            {
                throw new System.IndexOutOfRangeException("Index out of range.");
            }
            return values[row, col];
        }
        set
        {
            if (row < 0 || row >= 3 || col < 0 || col >= 3)
            {
                throw new System.IndexOutOfRangeException("Index out of range.");
            }
            values[row, col] = value;
        }
    }

    //Operators
    public static Matrix3x3 operator+(Matrix3x3 _matrixA, Matrix3x3 _matrixB)
    {
        Matrix3x3 result = new Matrix3x3();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[i, j] = _matrixA[i, j] + _matrixB[i, j];
            }
        }

        return result;
    }

    public static Matrix3x3 operator*(Matrix3x3 _matrix, float _scalar)
    {
        Matrix3x3 result = new Matrix3x3();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[i, j] = _matrix[i, j] * _scalar;
            }
        }

        return result;
    }

    public static Vector3 operator *(Matrix3x3 _matrix, Vector3 _vector)
    {
        return new Vector3(_matrix[0, 0] * _vector.x + _matrix[0, 1] * _vector.y + _matrix[0, 2] * _vector.z,
            _matrix[1, 0] * _vector.x + _matrix[1, 1] * _vector.y + _matrix[1, 2] * _vector.z,
            _matrix[2, 0] * _vector.x + _matrix[2, 1] * _vector.y + _matrix[2, 2] * _vector.z);
    }

    public static Matrix3x3 operator *(Matrix3x3 _matrixA, Matrix3x3 _matrixB)
    {
        Matrix3x3 result = new Matrix3x3();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[i, j] = 0;
                for (int k = 0; k < 3; k++)
                {
                    result[i, j] += _matrixA[i, k] * _matrixB[k, j];
                }
            }
        }

        return result;
    }

    //Matrix Specific Operations
    public float Determinant()
    {
        return values[0, 0] * (values[1, 1] * values[2, 2] - values[1, 2] * values[2, 1]) -
               values[0, 1] * (values[1, 0] * values[2, 2] - values[1, 2] * values[2, 0]) +
               values[0, 2] * (values[1, 0] * values[2, 1] - values[1, 1] * values[2, 0]);
    }

    public Matrix3x3 Transpose()
    {
        Matrix3x3 result = new Matrix3x3();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                result[i, j] = values[j, i];
            }
        }
        return result;
    }

    public Matrix3x3 Inverse()
    {
        float det = Determinant();

        if (det == 0)
        {
            throw new System.Exception("Matrix is singular and cannot be inverted.");
        }

        Matrix3x3 result = new Matrix3x3();
        result[0, 0] = (values[1, 1] * values[2, 2] - values[1, 2] * values[2, 1]) / det;
        result[0, 1] = (values[0, 2] * values[2, 1] - values[0, 1] * values[2, 2]) / det;
        result[0, 2] = (values[0, 1] * values[1, 2] - values[0, 2] * values[1, 1]) / det;
        result[1, 0] = (values[1, 2] * values[2, 0] - values[1, 0] * values[2, 2]) / det;
        result[1, 1] = (values[0, 0] * values[2, 2] - values[0, 2] * values[2, 0]) / det;
        result[1, 2] = (values[0, 2] * values[1, 0] - values[0, 0] * values[1, 2]) / det;
        result[2, 0] = (values[1, 0] * values[2, 1] - values[1, 1] * values[2, 0]) / det;
        result[2, 1] = (values[0, 1] * values[2, 0] - values[0, 0] * values[2, 1]) / det;
        result[2, 2] = (values[0, 0] * values[1, 1] - values[0, 1] * values[1, 0]) / det;

        return result;
    }

    private static bool checkIfValid(float[,] _values)
    {
        return (_values.GetLength(0) != 3 || _values.GetLength(1) != 3 || _values.GetLength(2) != 3);
    }
}

    

