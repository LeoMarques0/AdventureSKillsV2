using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Carousel : LayoutGroup
{

    #region Serialized Fields
    [SerializeField, Range(0, 5)]
    private float smoothness = 2.5f;
    #endregion
    #region Private Fields
    private float cellWidth = 0;
    private int columns = 0;
    #endregion
    #region Public Fields
    public Button[] arrowsBtns = new Button[2];
    public float columnIndex = 0;
    public int targetIndex = 0;

    public bool ready = false;
    public bool inUse = false;

    public PhotonView photonView;
    #endregion

    #region LayoutGroup Methods

    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        columns = transform.childCount;

        columnIndex = Mathf.Clamp(columnIndex, 0, columns - 1);

        float parentWidth = rectTransform.rect.width;

        cellWidth = parentWidth;

        CalculateObjectXPos();
    }

    public override void CalculateLayoutInputVertical()
    {
        
    }

    public override void SetLayoutHorizontal()
    {
        
    }

    public override void SetLayoutVertical()
    {
        
    }

    #endregion

    #region Private Methods

    private void CalculateObjectXPos()
    {
        int columnCount = 0;

        //var spacing = (m_Padding.left * columns) - (m_Padding.right);
        var firstXPos = cellWidth;
        //+ ((spacing * columns) - spacing);

        for (int i = 0; i < rectChildren.Count; i++)
        {
            columnCount = i % columns;

            var item = rectChildren[i];
            var xPos = (cellWidth * columnCount) + m_Padding.left - m_Padding.right / 2;
            var finalWidth = cellWidth - m_Padding.left / 2 - m_Padding.right / 2;
            //+ ((spacing * columns) - spacing);

            xPos -= firstXPos * columnIndex;

            SetChildAlongAxis(item, 0, xPos, finalWidth);
        }
    }

    #endregion

    #region Public Methods

    public void CallChangeColumn(int sumIndex)
    {
        if (PhotonNetwork.IsConnected)
            photonView.RPC("StartChangeColumn", RpcTarget.AllBuffered, sumIndex);
    }

    [PunRPC]
    public void StartChangeColumn(int sumIndex)
    {
        targetIndex += sumIndex;
        targetIndex = Mathf.Clamp(targetIndex, 0, columns - 1);

        StopAllCoroutines();
        StartCoroutine(ChangeColumn());
    }

    public IEnumerator ChangeColumn()
    {
        while(columnIndex != targetIndex)
        {
            columnIndex = Mathf.Lerp(columnIndex, targetIndex, smoothness * Time.deltaTime);
            CalculateObjectXPos();
            yield return null;
        }
    }

    #endregion
}
