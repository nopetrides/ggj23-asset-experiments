﻿using UnityEngine;

namespace FIMSpace.BonesStimulation
{
    public partial class BonesStimulator
    {

        public void UpdateMusclesLogics()
        {
            if (MovementMuscles > 0f)
                UpdateMovementMusclesWith();

            if (RotationSpaceMuscles > 0f)
                UpdateRotationSpaceMusclesWith();
        }


        public void UpdateMovementMusclesWith()
        {
            float mDelta = delta * MusclesSimulationSpeed;

            bool muscleColl = UseCollisions && MovementMusclesCollision;

            Bone bone = Bones[0];
            if (!muscleColl)
            {
                while (bone != null)
                {
                    if (MotionInfluence < 1f) bone.MotionMuscle.PositionMuscle.MotionInfluence(influenceOffset);
                    bone.MotionMuscle.UpdateElasticPosition(mDelta);
                    bone = bone.Child;
                }
            }
            else
            {
                while (bone != null)
                {
                    if (MotionInfluence < 1f) bone.MotionMuscle.PositionMuscle.MotionInfluence(influenceOffset);
                    bone.MotionMuscle.UpdateElasticPosition(mDelta);

                    if (bone.EnableCollisions)
                    {
                        Vector3 musclePos = bone.MotionMuscle.ProceduralPosition;
                        PushIfSegmentInsideCollider(bone, ref musclePos);
                        bone.MotionMuscle.OverrideProceduralPosition(musclePos);
                    }

                    bone = bone.Child;
                }
            }


            bone = Bones[0];
            while (bone != null)
            {
                bone.MotionMuscle.UpdateElasticRotation(MovementMuscles * GetEffectBlendWeight() * MusclesBlend.Evaluate(bone.Evaluation));
                bone = bone.Child;
            }
        }


        public void UpdateRotationSpaceMusclesWith()
        {
            float mDelta = delta * MusclesSimulationSpeed;
            float blend = RotationSpaceMuscles * GetEffectBlendWeight();

            Bone bone = Bones[0];

            if (UseEulerRotation)
            {
                while (bone != null)
                {
                    bone.EulerAnglesMuscle.Update(mDelta, bone.transform.eulerAngles);

                    float blendC = blend * MusclesBlend.Evaluate(bone.Evaluation);

                    Quaternion targetRot = Quaternion.Euler(bone.EulerAnglesMuscle.ProceduralEulerAngles);
                    //float angle = Quaternion.Angle(bone.transform.rotation, targetRot);

                    //if (angle > 10f) UnityEngine.Debug.Log("Angle = " + angle);
                    //if (angle > 45f)
                    //{

                    //}
                    //else
                    {
                        if (blendC >= 1f) bone.transform.rotation = targetRot;
                        else bone.transform.rotation = Quaternion.LerpUnclamped(bone.transform.rotation, targetRot, blendC);
                    }

                    bone = bone.Child;
                }
            }
            else
            {
                while (bone != null)
                {
                    if (EnsureRotation)
                        bone.RotationMuscle.UpdateEnsured(mDelta, bone.transform.rotation);
                    else
                        bone.RotationMuscle.Update(mDelta, bone.transform.rotation);

                    float blendC = blend * MusclesBlend.Evaluate(bone.Evaluation);

                    if (blendC >= 1f) bone.transform.rotation = bone.RotationMuscle.ProceduralRotation;
                    else bone.transform.rotation = Quaternion.LerpUnclamped(bone.transform.rotation, bone.RotationMuscle.ProceduralRotation, blendC);

                    bone = bone.Child;
                }
            }
        }

    }
}