/*
** Created by fengling
** DateTime:    2021-05-25 10:15:44
** Description: 飞行物
*/

using UnityEngine;
using System.Collections.Generic;
using Framework.Scripts.Utils;
using SRF;
using UnityEngine.Serialization;

namespace Framework.Scripts.Objects
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TrajectoryObject2D : MonoBehaviour
    {
        #region 参数

        // Padding value used to prevent the collider from overlapping the environment collider. Overlapped colliders don't work well with ray casts.
        private const float CCollider2DSpacing = 0.01f;

        /// <summary>
        /// Specifies how the object should bounce after hitting another collider.
        /// </summary>
        public enum BounceMode
        {
            None, // Do not bounce.
            Reflect, // Reflect according to the velocity.
            RandomReflect // Reflect in a random direction. This mode will make the object nonkinematic but for visual only objects such as shells this is preferred.
        }

        [FormerlySerializedAs("m_InitializeOnEnable")] [Tooltip("Should the component initialize when enabled?")] [SerializeField]
        protected bool mInitializeOnEnable = false;

        [FormerlySerializedAs("m_Mass")] [Tooltip("The mass of the object.")] [SerializeField]
        protected float mMass = 1;

        [FormerlySerializedAs("m_StartVelocityMultiplier")] [Tooltip("Multiplies the starting velocity by the specified value.")] [SerializeField]
        protected float mStartVelocityMultiplier = 1;

        [FormerlySerializedAs("m_GravityMagnitude")] [Tooltip("The amount of gravity to apply to the object.")] [Range(0, 40)] [SerializeField]
        protected float mGravityMagnitude = 0f;

        [FormerlySerializedAs("m_Speed")] [Tooltip("The movement speed.")] [SerializeField]
        protected float mSpeed = 1;

        [FormerlySerializedAs("m_RotationSpeed")] [Tooltip("The rotation speed.")] [SerializeField]
        protected float mRotationSpeed = 5;

        [FormerlySerializedAs("m_Damping")] [Tooltip("The amount of damping to apply to the movement.")] [Range(0, 1)] [SerializeField]
        protected float mDamping = 0.1f;

        [FormerlySerializedAs("m_RotationDamping")] [Tooltip("Amount of damping to apply to the torque.")] [Range(0, 1)] [SerializeField]
        protected float mRotationDamping = 0.1f;

        [FormerlySerializedAs("m_RotateInMoveDirection")] [Tooltip("Should the object rotate in the direction that it is moving?")] [SerializeField]
        protected bool mRotateInMoveDirection;

        [FormerlySerializedAs("m_SettleThreshold")]
        [Tooltip(
            "When the velocity and torque have a square magnitude value less than the specified value the object will be considered settled.")]
        [SerializeField]
        protected float mSettleThreshold;

        [FormerlySerializedAs("m_SidewaysSettleThreshold")]
        [Tooltip(
            "Specifies if the collider should settle on its side or upright. The higher the value the more likely the collider will settle on its side. " +
            "This is only used for CapsuleCollider2Ds and BoxCollider2Ds.")]
        [Range(0, 1)]
        [SerializeField]
        protected float mSidewaysSettleThreshold = 0.75f;

        [FormerlySerializedAs("m_StartSidewaysVelocityMagnitude")]
        [Tooltip(
            "Starts to rotate to the settle rotation when the velocity magnitude is less than the specified values.")]
        [SerializeField]
        protected float mStartSidewaysVelocityMagnitude = 3f;

        [FormerlySerializedAs("m_ImpactLayers")] [Tooltip("The layers that the object can collide with.")] [SerializeField]
        protected LayerMask mImpactLayers;

        [FormerlySerializedAs("m_ForceMultiplier")] [Tooltip("When a force is applied the multiplier will modify the magnitude of the force.")] [SerializeField]
        protected float mForceMultiplier = 40;

        [FormerlySerializedAs("m_BounceMode")] [Tooltip("Specifies how the object should bounce after hitting another collider.")] [SerializeField]
        protected BounceMode mBounceMode = BounceMode.Reflect;

        [FormerlySerializedAs("m_BounceMultiplier")]
        [Tooltip("If the object can bounce, specifies the multiplier to apply to the bounce velocity.")]
        [Range(0, 4)]
        [SerializeField]
        protected float mBounceMultiplier = 1;

        [FormerlySerializedAs("m_MaxCollisionCount")] [Tooltip("The maximum number of objects the projectile can collide with at a time.")] [SerializeField]
        protected int mMaxCollisionCount = 5;

        [FormerlySerializedAs("m_MaxPositionCount")] [Tooltip("The maximum number of positions any single curve amplitude can contain.")] [SerializeField]
        protected int mMaxPositionCount = 150;

        public float Mass
        {
            get { return mMass; }
            set { mMass = value; }
        }

        public float StartVelocityMultiplier
        {
            get { return mStartVelocityMultiplier; }
            set { mStartVelocityMultiplier = value; }
        }

        public float GravityMagnitude
        {
            get { return mGravityMagnitude; }
            set { mGravityMagnitude = value; }
        }

        public float Speed
        {
            get { return mSpeed; }
            set { mSpeed = value; }
        }

        public float RotationSpeed
        {
            get { return mRotationSpeed; }
            set { mRotationSpeed = value; }
        }

        public float Damping
        {
            get { return mDamping; }
            set { mDamping = value; }
        }

        public float RotationDamping
        {
            get { return mRotationDamping; }
            set { mRotationDamping = value; }
        }

        public bool RotateInMoveDirection
        {
            get { return mRotateInMoveDirection; }
            set { mRotateInMoveDirection = value; }
        }

        public float SettleThreshold
        {
            get { return mSettleThreshold; }
            set { mSettleThreshold = value; }
        }

        public float SidewaysSettleThreshold
        {
            get { return mSidewaysSettleThreshold; }
            set { mSidewaysSettleThreshold = value; }
        }

        public float StartSidewaysVelocityMagnitude
        {
            get { return mStartSidewaysVelocityMagnitude; }
            set { mStartSidewaysVelocityMagnitude = value; }
        }

        public LayerMask ImpactLayers
        {
            get { return mImpactLayers; }
            set { mImpactLayers = value; }
        }

        public float ForceMultiplier
        {
            get { return mForceMultiplier; }
            set { mForceMultiplier = value; }
        }

        public BounceMode Bounce
        {
            get { return mBounceMode; }
            set { mBounceMode = value; }
        }

        public float BounceMultiplier
        {
            get { return mBounceMultiplier; }
            set { mBounceMultiplier = value; }
        }

        protected GameObject MGameObject;
        protected Transform MTransform;
        protected Collider2D MCollider2D;
        protected GameObject MOriginator;
        protected Transform MOriginatorTransform;
        private LineRenderer _mLineRenderer;

        private RaycastHit2D _mRaycastHit;
        private Collider2D[] _mCollider2DHit;
        private List<Vector3> _mPositions;

        private Vector3 _mGravity;
        protected Vector3 MNormalizedGravity;
        protected Vector3 MVelocity;
        protected Vector3 MTorque;
        private bool _mDeterminedRotation;
        private bool _mSettleSideways;
        private bool _mOriginatorCollisionCheck;

        private float _mTimeScale;
        private bool _mAutoDisable;
        private bool _mMovementSettled;
        private bool _mRotationSettled;
        private bool _mInCollision;
        private bool _mBounced;

        private Transform _mPlatform;
        private Vector3 _mPlatformRelativePosition;
        private Quaternion _mPrevPlatformRotation;

        public GameObject Originator
        {
            get { return MOriginator; }
        }

        public Vector3 Velocity
        {
            get { return MVelocity; }
        }

        public Vector3 Torque
        {
            get { return MTorque; }
        }

        public LineRenderer LineRenderer
        {
            get { return _mLineRenderer; }
        }

        protected bool AutoDisable
        {
            set { _mAutoDisable = value; }
        }

        #endregion

        /// <summary>
        /// Initialize the defualt values.
        /// </summary>
        protected virtual void Awake()
        {
            mImpactLayers = 1 << LayerMask.NameToLayer("Wall");
            // The movement will be controlled by the TrajectoryObject.
            var rigidbody = gameObject.GetComponentOrAdd<Rigidbody2D>();
            if (rigidbody != null)
            {
                rigidbody.gravityScale = 0;
                rigidbody.isKinematic = true;
            }

            // The object may want to play audio.
            // var hasActiveAudioClipSet = false;

            enabled = mInitializeOnEnable;
        }

        /// <summary>
        /// The component has been enabled.
        /// </summary>
        protected virtual void OnEnable()
        {
            if (mInitializeOnEnable)
            {
                InitializeComponentReferences();
                Initialize(Vector3.zero, Vector3.zero, null, false, -MTransform.up);
            }
        }

        /// <summary>
        /// Initializes the object with the specified velocity and torque.
        /// </summary>
        /// <param name="velocity">The starting velocity.</param>
        /// <param name="torque">The starting torque.</param>
        /// <param name="originator">The object that instantiated the trajectory object.</param>
        public virtual void Initialize(Vector3 velocity, Vector3 torque, GameObject originator)
        {
            Initialize(velocity, torque, originator, true);
        }

        /// <summary>
        /// Initializes the object with the specified velocity and torque.
        /// </summary>
        /// <param name="velocity">The starting velocity.</param>
        /// <param name="torque">The starting torque.</param>
        /// <param name="originator">The object that instantiated the trajectory object.</param>
        /// <param name="originatorCollisionCheck">Should a collision check against the originator be performed?</param>
        public virtual void Initialize(Vector3 velocity, Vector3 torque, GameObject originator,
            bool originatorCollisionCheck)
        {
            Initialize(velocity, torque, originator, originatorCollisionCheck, Vector3.down);
        }

        /// <summary>
        /// Initializes the object with the specified velocity and torque.
        /// </summary>
        /// <param name="velocity">The starting velocity.</param>
        /// <param name="torque">The starting torque.</param>
        /// <param name="originator">The object that instantiated the trajectory object.</param>
        /// <param name="originatorCollisionCheck">Should a collision check against the originator be performed?</param>
        /// <param name="defaultNormalizedGravity">The normalized gravity direction if a character isn't specified for the originator.</param>
        public virtual void Initialize(Vector3 velocity, Vector3 torque, GameObject originator,
            bool originatorCollisionCheck, Vector3 defaultNormalizedGravity)
        {
            InitializeComponentReferences();

            MVelocity = velocity / mMass * mStartVelocityMultiplier;
            MTorque = torque;
            if (originator != null && originator != MOriginator)
            {
                MOriginator = originator;
                MOriginatorTransform = MOriginator.transform;
            }
            else
            {
                MNormalizedGravity = defaultNormalizedGravity;
                _mTimeScale = 1;
                MOriginatorTransform = null;
            }

            _mGravity = MNormalizedGravity * mGravityMagnitude;
            _mOriginatorCollisionCheck = originatorCollisionCheck && MOriginator != null;

            _mPlatform = null;
            _mMovementSettled = _mRotationSettled = false;
            _mInCollision = false;
            _mBounced = false;
            if (MCollider2D != null)
            {
                MCollider2D.enabled = true;
            }

            enabled = true;

            // Set the layer to prevent the current object from getting in the way of the casts.
            var previousLayer = MGameObject.layer;
            MGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");

            // The object could start in a collision state.
            if (_mOriginatorCollisionCheck && OverlapCast(MTransform.position, MTransform.rotation))
            {
                OnCollision(null);
                if (mBounceMode == BounceMode.None)
                {
                    _mMovementSettled = _mRotationSettled = true;
                }
                else
                {
                    // Update the velocity to the reflection direction. Use the originator's forward direction as the normal because the actual collision point is not determined.
                    MVelocity = Vector3.Reflect(MVelocity, -MOriginatorTransform.forward) * mBounceMultiplier;
                }
            }

            MGameObject.layer = previousLayer;
        }

        /// <summary>
        /// Retruns true if any objects are overlapping with the Trajectory Object.
        /// </summary>
        /// <param name="position">The position of the cast.</param>
        /// <param name="rotation">The rotation of the cast.</param>
        /// <returns>True if any objects are overlapping with the Trajectory Object.</returns>
        private bool OverlapCast(Vector3 position, Quaternion rotation)
        {
            // No need to do a cast if the originator is null.
            if (MOriginatorTransform == null)
            {
                return false;
            }

            int hit = 0;

            if (MCollider2D is CircleCollider2D)
            {
                var sphereCollider2D = MCollider2D as CircleCollider2D;
                hit = Physics2D.OverlapCircleNonAlloc(
                    MathUtility.TransformPoint(position, MTransform.rotation, sphereCollider2D.offset),
                    sphereCollider2D.radius * MathUtility.ColliderRadiusMultiplier(sphereCollider2D), _mCollider2DHit,
                    mImpactLayers);
            }
            else if (MCollider2D is CapsuleCollider2D)
            {
                var capsuleCollider2D = MCollider2D as CapsuleCollider2D;

                Vector3 startEndCap, endEndCap;
                MathUtility.CapsuleColliderEndCaps(capsuleCollider2D, position, rotation, out startEndCap,
                    out endEndCap);
                hit = Physics2D.OverlapCapsuleNonAlloc(startEndCap, endEndCap,
                    capsuleCollider2D.direction,
                    capsuleCollider2D.transform.rotation.z,
                    _mCollider2DHit,
                    mImpactLayers);
            }
            else if (MCollider2D is BoxCollider2D)
            {
                var boxCollider2D = MCollider2D as BoxCollider2D;
                hit = Physics2D.OverlapBoxNonAlloc(
                    MathUtility.TransformPoint(position, MTransform.rotation, boxCollider2D.offset),
                    Vector3.Scale(boxCollider2D.size, boxCollider2D.transform.lossyScale) / 2,
                    boxCollider2D.transform.rotation.z,
                    _mCollider2DHit,
                    mImpactLayers);
            }

            if (hit > 0)
            {
                // The TrajectoryObject is only in an overlap state if the object is overlapping a non-character or camera collider.
                for (int i = 0; i < hit; ++i)
                {
                    if (!_mCollider2DHit[i].transform.IsChildOf(MOriginatorTransform)
#if FIRST_PERSON_CONTROLLER
                    // The object should not hit any colliders who are a child of the camera.
                    && m_Collider2DHit[i].transform.gameObject.GetCachedParentComponent<FirstPersonController.Character.FirstPersonObjects>() == null
#endif
                    )
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes the local component references.
        /// </summary>
        protected void InitializeComponentReferences()
        {
            if (MGameObject != null)
            {
                return;
            }

            MGameObject = gameObject;
            MTransform = transform;
            var colliders = GetComponents<Collider2D>();
            for (int i = 0; i < colliders.Length; ++i)
            {
                // The collider cannot be a triger.
                if (colliders[i].isTrigger)
                {
                    continue;
                }

                // The collider has to be of the correct type.
                if (!(colliders[i] is CircleCollider2D) && !(colliders[i] is CapsuleCollider2D) &&
                    !(colliders[i] is BoxCollider2D))
                {
                    continue;
                }

                MCollider2D = colliders[i];
                break;
            }

            _mLineRenderer = GetComponent<LineRenderer>();
            _mCollider2DHit = new Collider2D[mMaxCollisionCount];
        }

        /// <summary>
        /// Move and rotate the object according to a parabolic trajectory.
        /// </summary>
        protected virtual void FixedUpdate()
        {
            // Update the position.
            var position = MTransform.position;
            var rotation = MTransform.rotation;

            // Set the layer to prevent the current object from getting in the way of the casts.
            var previousLayer = MGameObject.layer;
            MGameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
            
            if (!Move(ref position, rotation))
            {
                // If the object collided with another object then Move should be called one more time so the reflected velocity is used.
                // If the second Move method is not called then the object would wait a tick before it is moved.
                Move(ref position, rotation);
            }
            
            // The object may have been disabed within OnCollision. Do not do any more updates for a disabled object.
            if (enabled)
            {
                if (_mPlatform != null)
                {
                    position += (_mPlatform.TransformPoint(_mPlatformRelativePosition) - position);
                    _mPlatformRelativePosition = _mPlatform.InverseTransformPoint(position);
                }
                
                MTransform.position = position;

                // Update the rotation.
                Rotate(position, ref rotation);
                if (_mPlatform != null)
                {
                    rotation *= (_mPlatform.rotation * Quaternion.Inverse(_mPrevPlatformRotation));
                    _mPrevPlatformRotation = _mPlatform.rotation;
                }

                MTransform.rotation = rotation;
            }

            MGameObject.layer = previousLayer;

            // If both the position and rotation are done making changes then the component can be disabled.
            if (_mAutoDisable && _mMovementSettled && _mRotationSettled)
            {
                enabled = false;
            }
        }

        /// <summary>
        /// Move the object based on the current velocity.
        /// </summary>
        /// <param name="position">The current position of the object. Passed by reference so the updated position can be set.</param>
        /// <param name="rotation">The current rotation of the object.</param>
        /// <returns>True if the position was updated or the movement has settled.</returns>
        private bool Move(ref Vector3 position, Quaternion rotation)
        {
            // The object can't move if the movement and rotation has settled.
            if (_mMovementSettled && _mRotationSettled && mSettleThreshold > 0)
            {
                return true;
            }

            // Stop moving if the velocity is less than a minimum threshold and the object is on the ground.
            if (MVelocity.sqrMagnitude < mSettleThreshold && _mRotationSettled)
            {
                // The object should be on the ground before the object has settled.
                if (SingleCast(position, rotation, MNormalizedGravity * CCollider2DSpacing))
                {
                    _mMovementSettled = true;
                    return true;
                }
            }

            var deltaTime = _mTimeScale * Time.fixedDeltaTime * Time.timeScale;
            // todo 速度衰减添加开关
            // The object hasn't settled yet - move based on the velocity.
            MVelocity += _mGravity * deltaTime;
            MVelocity *= Mathf.Clamp01(1 - mDamping * deltaTime);

            // If the object hits an object then it should either reflect off of that object or stop moving.
            var targetPosition = position + MVelocity * mSpeed * deltaTime;
            var direction = targetPosition - position;
            if (SingleCast(position, rotation, direction))
            {
                if (_mRaycastHit.transform.gameObject.layer == LayerMask.NameToLayer("MovingPlatform"))
                {
                    if (_mRaycastHit.transform != _mPlatform)
                    {
                        _mPlatform = _mRaycastHit.transform;
                        _mPlatformRelativePosition = _mPlatform.InverseTransformPoint(position);
                        _mPrevPlatformRotation = _mPlatform.rotation;
                    }
                }
                else
                {
                    _mPlatform = null;
                }

                // If the object has settled but not disabled a collision will occur every frame. Prevent the effects from playing because of this.
                if (!_mInCollision)
                {
                    _mInCollision = true;

                    OnCollision(_mRaycastHit);
                }

                if (mBounceMode != BounceMode.None)
                {
                    Vector3 velocity;
                    if (mBounceMode == BounceMode.RandomReflect)
                    {
                        // Add ramdomness to the bounce.
                        // This mode should not be used over the network unless it doesn't matter if the object is synchronized (such as a shell).
                        velocity = Quaternion.AngleAxis(Random.Range(-70, 70), _mRaycastHit.normal) * MVelocity;
                    }
                    else
                    {
                        // Reflect.
                        velocity = MVelocity;
                    }

                    if (MVelocity.magnitude < mStartSidewaysVelocityMagnitude)
                    {
                        _mMovementSettled = true;
                    }

                    _mBounced = true;
                    return false;
                }
                else
                {
                    MVelocity = Vector3.zero;
                    MTorque = Vector3.zero;
                    _mMovementSettled = true;
                    enabled = false;
                    return true;
                }
            }
            else
            {
                _mPlatform = null;
                _mInCollision = false;
            }

            position = targetPosition;
            return true;
        }

        /// <summary>
        /// The object has collided with another object.
        /// </summary>
        /// <param name="hit">The RaycastHit of the object. Can be null.</param>
        protected virtual void OnCollision(RaycastHit2D? hit)
        {
            if (hit != null && hit.HasValue)
            {
                // A Rigidbody2D should be affected by the impact.
                if (hit.Value.rigidbody != null)
                {
                    hit.Value.rigidbody.AddForceAtPosition(MVelocity, hit.Value.point);
                }
            }
        }

        /// <summary>
        /// Does a cast in in the specified direction.
        /// </summary>
        /// <param name="position">The position of the cast.</param>
        /// <param name="rotation">The rotation of the cast.</param>
        /// <param name="direction">The direction of the cast.</param>
        /// <returns>The number of hit results.</returns>
        private bool SingleCast(Vector3 position, Quaternion rotation, Vector2 direction)
        {
            switch (MCollider2D)
            {
                // todo CapsuleCollider2D 碰撞检测
                /*else if (m_Collider2D is CapsuleCollider2D)
            {
                var capsuleCollider2D = m_Collider2D as CapsuleCollider2D;
                Vector3 startEndCap, endEndCap;
                MathUtility.CapsuleColliderEndCaps(capsuleCollider2D, position, rotation, out startEndCap, out endEndCap);
                var radius = capsuleCollider2D.size.x * MathUtility.ColliderRadiusMultiplier(capsuleCollider2D);
                hit = Physics2D.CapsuleCast(startEndCap, endEndCap, radius, direction.normalized, out m_RaycastHit,
                    direction.magnitude + c_Collider2DSpacing, m_ImpactLayers, QueryTriggerInteraction.Ignore);
            }*/
                case CircleCollider2D sphereCollider2D:
                {
                    float colliderRadiusMultiplier =
                        sphereCollider2D.radius * MathUtility.ColliderRadiusMultiplier(sphereCollider2D);
                    _mRaycastHit = Physics2D.CircleCast(position,
                        colliderRadiusMultiplier,
                        direction.normalized, sphereCollider2D.radius, mImpactLayers);
                    break;
                }
                case BoxCollider2D mCollider2D:
                {
                    var boxCollider2D = mCollider2D;
                    _mRaycastHit = Physics2D.BoxCast(MTransform.TransformPoint(boxCollider2D.offset),
                        boxCollider2D.size,
                        0,
                        direction.normalized, 0, mImpactLayers);
                    break;
                }
                default:
                    // No collider attached.
                    _mRaycastHit = Physics2D.Raycast(position, direction.normalized, 0,
                        mImpactLayers);
                    break;
            }

            // The object should not collide with the originator to prevent the character from hitting themself.
            if (_mOriginatorCollisionCheck && MOriginatorTransform != null)
            {
                if (_mRaycastHit && (_mRaycastHit.transform.IsChildOf(MOriginatorTransform)
#if FIRST_PERSON_CONTROLLER
                    // The object should not hit any colliders who are a child of the camera.
                    || m_RaycastHit.transform.gameObject.GetCachedParentComponent<FirstPersonController.Character.FirstPersonObjects>() != null
#endif
                    ))
                {
                }
                else
                {
                    _mOriginatorCollisionCheck = false;
                }
            }

            return _mRaycastHit.transform != null;
        }

        /// <summary>
        /// Rotate the object based on the current torque.
        /// </summary>
        /// <param name="position">The current position of the object.</param>
        /// <param name="rotation">The current rotation of the object. Passed by reference so the updated rotation can be set.</param>
        private void Rotate(Vector3 position, ref Quaternion rotation)
        {
            // The object should rotate to the desired direction after it has bounced and the rotation has settled.
            if ((mBounceMode == BounceMode.None || _mBounced) &&
                (MTorque.sqrMagnitude < mSettleThreshold || _mMovementSettled)
            )
            {
                if (MCollider2D is CapsuleCollider2D || MCollider2D is BoxCollider2D)
                {
                    if (!_mRotationSettled)
                    {
                        var up = -MNormalizedGravity;
                        var normal = up;
                        if (SingleCast(position, rotation, MNormalizedGravity * CCollider2DSpacing))
                        {
                            normal = _mRaycastHit.normal;
                        }

                        var dot = Mathf.Abs(Vector3.Dot(normal, rotation * Vector3.up));
                        if (dot > 0.0001 && dot < 0.9999)
                        {
                            // Allow the object to be force rotated to a rotation based on the sideways settle threshold. This works well with bullet
                            // shells to allow them to settle upright instead of always settling on their side.
                            var localRotation = MathUtility
                                .InverseTransformQuaternion(Quaternion.LookRotation(Vector3.forward, up), rotation)
                                .eulerAngles;
                            if (!_mDeterminedRotation)
                            {
                                _mSettleSideways = dot < mSidewaysSettleThreshold;
                                _mDeterminedRotation = true;
                            }

                            localRotation.x = 0;
                            if (_mSettleSideways)
                            {
                                // The collider should settle on its side.
                                localRotation.z = Mathf.Abs(MathUtility.ClampInnerAngle(localRotation.z)) < 90
                                    ? 0
                                    : 180;
                            }
                            else
                            {
                                // The collider should settle upright.
                                localRotation.z = MathUtility.ClampInnerAngle(localRotation.z) < 0 ? 270 : 90;
                            }

                            var target = MathUtility.TransformQuaternion(Quaternion.LookRotation(Vector3.forward, up),
                                Quaternion.Euler(localRotation));
                            var deltaTime = _mTimeScale * Time.fixedDeltaTime * Time.timeScale;
                            rotation = Quaternion.Slerp(rotation, target, mRotationSpeed * deltaTime);
                        }
                        else
                        {
                            // The object has finished rotating.
                            MTorque = Vector3.zero;
                            _mRotationSettled = true;
                        }
                    }
                }
                else
                {
                    MTorque = Vector3.zero;
                    _mRotationSettled = true;
                }
            }

            // Determine the new rotation.
            if (mRotateInMoveDirection && MVelocity.sqrMagnitude > 0)
            {
                rotation = Quaternion.LookRotation(MVelocity.normalized, -_mGravity);
            }

            MTorque *= Mathf.Clamp01(1 - mRotationDamping);
            var targetRotation = rotation * Quaternion.Euler(MTorque);

            // Do not rotate if the collider would intersect with another object. A CircleCollider2D does not need this check.
            var hitCount = 0;
            if (MCollider2D is CapsuleCollider2D)
            {
                Vector3 startEndCap, endEndCap;
                var capsuleCollider2D = MCollider2D as CapsuleCollider2D;
                MathUtility.CapsuleColliderEndCaps(capsuleCollider2D, position, targetRotation, out startEndCap,
                    out endEndCap);
                hitCount = Physics2D.OverlapCapsuleNonAlloc(startEndCap, endEndCap,
                    capsuleCollider2D.direction,
                    capsuleCollider2D.size.x * MathUtility.CapsuleColliderHeightMultiplier(capsuleCollider2D),
                    _mCollider2DHit,
                    mImpactLayers);
            }
            else if (MCollider2D is BoxCollider2D)
            {
                var boxCollider2D = MCollider2D as BoxCollider2D;
                float angle;
                MTransform.rotation.ToAngleAxis(out angle, out Vector3 back);
                hitCount = Physics2D.OverlapBoxNonAlloc(
                    MathUtility.TransformPoint(position, MTransform.rotation, boxCollider2D.offset),
                    Vector3.Scale(boxCollider2D.size, boxCollider2D.transform.lossyScale) / 2, angle,
                    _mCollider2DHit,
                    mImpactLayers);
            }

            // Apply the rotation if the rotation doesnt intersect any object.
            if (hitCount == 0)
            {
                rotation = targetRotation;
            }
        }
    }
}