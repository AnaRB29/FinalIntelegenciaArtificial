using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TileSelector))]
public class TileSimpleAnimator : MonoBehaviour
{
    [SerializeField] private Tilemap tileMap;
    
    private TileSelector _selector;
    private Dictionary<Vector3Int, AnimationHandler> _activeTween = new();

    private void Awake()
    {
        _selector = GetComponent<TileSelector>();
    }

    private void OnSelectedTile(CustomTile tile, Vector3Int pos)
    {
        if (_activeTween.ContainsKey(pos)) return;

        //StartCoroutine(AnimationTest(pos));

        var cells = PathFindingAlgorithm.FillLimited(pos, 1, tileMap);
        StartCoroutine(Anim2(cells));
    }

    private IEnumerator Anim2(List<Vector3Int> cells)
    {
        for (var i = 0; i < cells.Count - 1; i++)
        {
            StartCoroutine(AnimationTest(cells[i]));
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator AnimationTest(Vector3Int cell)
    {
        var animHandler = new AnimationHandler();
        _activeTween.Add(cell, animHandler);
        animHandler.MovePos(new Vector3(0,0,0), 0.5f);
        yield return WaitTween(animHandler.tweenPos, animHandler, cell);
        animHandler.MovePos(animHandler.originalPos, 0.5f);
        yield return WaitTween(animHandler.tweenPos, animHandler, cell);
        
        _activeTween.Remove(cell);
    }

    private IEnumerator WaitTween(Tween tween, AnimationHandler animHandler, Vector3Int cell)
    {
        yield return new WaitUntil(() =>
        {
            SetMatrix(animHandler,cell);
            return tween == null || tween.IsComplete() || !tween.IsPlaying() ;
        });
    }

    private void SetMatrix(AnimationHandler animHandler, Vector3Int cell)
    {
        var matrix = Matrix4x4.TRS(animHandler.pos, animHandler.rot, animHandler.scale);
        tileMap.SetTransformMatrix(cell, matrix);
    }

    private class AnimationHandler
    {
        public readonly Vector3 originalPos = new Vector3(0, -0.12f, 0);
        public readonly Vector3 originalRot = Vector3.zero;
        public readonly Vector3 originalScale = Vector3.one;

        public Vector3 pos;
        public Quaternion rot;
        public Vector3 scale;

        public Tween tweenPos = null;
        public Tween tweenRot = null;
        public Tween tweenScale = null;

        private bool _interruptTweens;

        public AnimationHandler(bool interruptTweens = false)
        {
            pos = originalPos;
            rot = Quaternion.Euler(originalRot);
            scale = originalScale;

            _interruptTweens = interruptTweens;
        }

        public void MovePos(Vector3 toPos, float seconds)
        {
            if(tweenPos != null && tweenPos.active)
            {
                if (_interruptTweens) tweenPos.Kill();
                else return;
            }

            tweenPos = DOTween.To(() => pos, value => pos = value, toPos, seconds)
                .OnComplete(() => tweenPos = null);
        }

        public void MoveRot(Vector3 toRot, float seconds)
        {
            if (tweenRot != null && tweenRot.active)
            {
                if (_interruptTweens) tweenRot.Kill();
                else return;
            }
            tweenRot = DOTween.To(() => rot, value => rot = value, toRot, seconds)
                .OnComplete(() => tweenRot = null);
        }

        public void MoveScale(Vector3 toScale, float seconds)
        {
            if (tweenScale != null && tweenScale.active)
            {
                if (_interruptTweens) tweenScale.Kill();
                else return;
            }

            tweenScale = DOTween.To(() => scale, value => scale = value, scale, seconds)
                .OnComplete(() => tweenScale = null);
        }
    }


    private void OnEnable()
    {
        _selector.OnSelectTile += OnSelectedTile;
    }

    private void OnDisable()
    {
        _selector.OnSelectTile -= OnSelectedTile;
    }
}
