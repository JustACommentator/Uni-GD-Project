using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.ActorSystem
{
    public class RPlayerFacialExpressionHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private MeshRenderer leftEyeRenderer = null;
        [SerializeField] private MeshRenderer rightEyeRenderer = null;
        [SerializeField] private MeshRenderer mouthRenderer = null;

        [Header("Values")]
        [SerializeField] private SPlayerExpression happyExpression = new SPlayerExpression();
        [SerializeField] private SPlayerExpression dizzyExpression = new SPlayerExpression();
        [SerializeField] private SPlayerExpression forcedExpression = new SPlayerExpression();
        [SerializeField] private SPlayerExpression angryExpression = new SPlayerExpression();

        public void SetExpression(SPlayerExpression expression)
        {
            leftEyeRenderer.material = expression.leftEyeMaterial;
            rightEyeRenderer.material = expression.rightEyeMaterial;
            mouthRenderer.material = expression.mouthMaterial;
        }

        public void SetExpressionToHappy()
        {
            SetExpression(happyExpression);            
        }

        public void SetExpressionToDizzy()
        {
            SetExpression(dizzyExpression);
        }

        public void SetExpressionToForced()
        {
            SetExpression(forcedExpression);
        }

        public void SetExpressionToAngry()
        {
            SetExpression(angryExpression);
        }
    }

    [Serializable]
    public struct SPlayerExpression
    {
        public Material leftEyeMaterial;
        public Material rightEyeMaterial;
        public Material mouthMaterial;
    }
}