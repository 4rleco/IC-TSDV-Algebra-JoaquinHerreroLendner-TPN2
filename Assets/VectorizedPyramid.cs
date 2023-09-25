using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorizedPyramid : MonoBehaviour
{
    LineRenderer firstVectorLine;
    LineRenderer secondVectorLine;
    LineRenderer thirdVectorLine;

    LineRenderer pyramidSideLine;
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] Material xMat;
    [SerializeField] Material yMat;
    [SerializeField] Material zMat;
    [SerializeField] Material pyramidMat;

    [SerializeField] float segmentQty;
    float segmentSize;

    Vector3 initialVector;
    Vector3 secondVector;
    Vector3 thirdVector;


    public float faceSum = 0;
    public float faceAreaSum = 0;
    public float pyramidVolume = 0;


    void Start()
    {
        SetVectors();
        BuildPyramidLineRender();
    }

    void SetVectors()
    {
        segmentSize = 1 / segmentQty;
        initialVector = new Vector3(Random.Range(0, 11), Random.Range(0, 11), Random.Range(0, 11));
        secondVector = new Vector3(initialVector.y, initialVector.x * -1, initialVector.z);

        thirdVector = new Vector3((initialVector.y * segmentSize * secondVector.z * segmentSize) - (initialVector.z * segmentSize * secondVector.y * segmentSize),
                                           ((initialVector.x * segmentSize * secondVector.z * segmentSize) - (initialVector.z * segmentSize * secondVector.x * segmentSize)) * -1,
                                           (initialVector.x * segmentSize * secondVector.y * segmentSize) - (initialVector.y * segmentSize * secondVector.x * segmentSize));


        firstVectorLine = Instantiate(linePrefab);
        secondVectorLine = Instantiate(linePrefab);
        thirdVectorLine = Instantiate(linePrefab);
        firstVectorLine.material = xMat;
        secondVectorLine.material = yMat;
        thirdVectorLine.material = zMat;
        firstVectorLine.positionCount = 2;
        firstVectorLine.SetPosition(1, initialVector);
        secondVectorLine.positionCount = 2;
        secondVectorLine.SetPosition(1, secondVector);
        thirdVectorLine.positionCount = 2;
        thirdVectorLine.SetPosition(1, thirdVector);
    }

    void BuildPyramidLineRender()
    {
        Vector3 origin = new Vector3(0, 0, 0);

        Vector3 originPos = origin;
        Vector3 firstVectorPos = initialVector;
        Vector3 secondVectorPos = secondVector;
        Vector3 crossVectorPos = thirdVector;
        Vector3 lastVerticePos = firstVectorPos + secondVectorPos;

        Vector3 displacementX = new Vector3(initialVector.x * segmentSize, initialVector.y * segmentSize, initialVector.z * segmentSize);
        Vector3 displacementY = new Vector3(secondVector.x * segmentSize, secondVector.y * segmentSize, secondVector.z * segmentSize);

        Vector3 upRightDisplacement = displacementX + displacementY;
        Vector3 upLeftDisplacement = -displacementX + displacementY;
        Vector3 downRightDisplacement = displacementX - displacementY;
        Vector3 downLeftDisplacement = -displacementX - displacementY;


        float heightMagnitude = Mathf.Sqrt(Mathf.Pow(thirdVector.x, 2) + Mathf.Pow(thirdVector.y, 2) + Mathf.Pow(thirdVector.z, 2));

        for (int i = 0; i < (1 / segmentSize) / 2; i++)
        {
            DrawLine(pyramidSideLine, originPos + (upRightDisplacement * i), originPos + (upRightDisplacement * i) + crossVectorPos);
            DrawLine(pyramidSideLine, firstVectorPos + (upLeftDisplacement * i), firstVectorPos + (upLeftDisplacement * i) + crossVectorPos);
            DrawLine(pyramidSideLine, secondVectorPos + (downRightDisplacement * i), secondVectorPos + (downRightDisplacement * i) + crossVectorPos);
            DrawLine(pyramidSideLine, lastVerticePos + (downLeftDisplacement * i), lastVerticePos + (downLeftDisplacement * i) + crossVectorPos);

            DrawLine(pyramidSideLine, originPos + (upRightDisplacement * i), firstVectorPos + (upLeftDisplacement * i));
            DrawLine(pyramidSideLine, originPos + (upRightDisplacement * i), secondVectorPos + (downRightDisplacement * i));
            DrawLine(pyramidSideLine, lastVerticePos + (downLeftDisplacement * i), firstVectorPos + (upLeftDisplacement * i));
            DrawLine(pyramidSideLine, lastVerticePos + (downLeftDisplacement * i), secondVectorPos + (downRightDisplacement * i));

            faceSum += heightMagnitude * 4;

            // vector3 a - vector3 b 
            Vector3 sideLength = (firstVectorPos + (upLeftDisplacement * i)) - (originPos + (upRightDisplacement * i));
            // Magnitude of the previous substraction
            float sideMagnitude = Mathf.Sqrt(Mathf.Pow(sideLength.x, 2) + Mathf.Pow(sideLength.y, 2) + Mathf.Pow(sideLength.z, 2));

            faceSum += sideMagnitude * 8;

            float baseArea = sideMagnitude * sideMagnitude;

            faceAreaSum += sideMagnitude * heightMagnitude * 4;

            if (i == 0 || i == (1 / segmentSize) / 2 - 1)
            {
                faceAreaSum += baseArea;
                if (i == 0)
                    Debug.Log(baseArea);
            }

            Vector3 nextSideLength = (firstVectorPos + (upLeftDisplacement * (i + 1))) - (originPos + (upRightDisplacement * (i + 1)));
            float nextSideMagnitude = Mathf.Sqrt(Mathf.Pow(nextSideLength.x, 2) + Mathf.Pow(nextSideLength.y, 2) + Mathf.Pow(nextSideLength.z, 2));

            float nextBaseArea = nextSideMagnitude * nextSideMagnitude;
            Debug.Log(baseArea + " " + nextBaseArea);
            faceAreaSum += baseArea - nextBaseArea;

            pyramidVolume += baseArea * heightMagnitude;

            originPos += crossVectorPos;
            firstVectorPos += crossVectorPos;
            secondVectorPos += crossVectorPos;
            lastVerticePos += crossVectorPos;

            DrawLine(pyramidSideLine, originPos + (upRightDisplacement * i), firstVectorPos + (upLeftDisplacement * i));
            DrawLine(pyramidSideLine, originPos + (upRightDisplacement * i), secondVectorPos + (downRightDisplacement * i));
            DrawLine(pyramidSideLine, lastVerticePos + (downLeftDisplacement * i), firstVectorPos + (upLeftDisplacement * i));
            DrawLine(pyramidSideLine, lastVerticePos + (downLeftDisplacement * i), secondVectorPos + (downRightDisplacement * i));
        }
    }

    void DrawLine(LineRenderer line, Vector3 firstPos, Vector3 secondPos)
    {
        line = Instantiate(linePrefab, transform);
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.material = pyramidMat;

        line.positionCount = 2;
        line.SetPosition(0, firstPos);
        line.SetPosition(1, secondPos);
    }
}
