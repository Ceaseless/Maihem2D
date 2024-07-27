using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Scripting;

#if UNITY_EDITOR
namespace Editor
{
    [UnityEditor.InitializeOnLoad]
#endif
    [Preserve]
    [DisplayStringFormat("{up}/{left}/{down}/{right}")]
    // ReSharper disable once InconsistentNaming
    public class WASDComposite : InputBindingComposite<Vector2> {
 
        // NOTE: This is a modified copy of Vector2Composite
        // (Source: https://forum.unity.com/threads/using-last-key-pressed-in-a-composite-2d-vector.1109609/) 
 
        [InputControl(layout = "Button")]
        public int Up = 0;
        [InputControl(layout = "Button")]
        public int Down = 0;
        [InputControl(layout = "Button")]
        public int Left = 0;
        [InputControl(layout = "Button")]
        public int Right = 0;
 
        private bool _upPressedLastFrame;
        private bool _downPressedLastFrame;
        private bool _leftPressedLastFrame;
        private bool _rightPressedLastFrame;
        private float _upPressTimestamp;
        private float _downPressTimestamp;
        private float _leftPressTimestamp;
        private float _rightPressTimestamp;

        private float _horizontalPressTimestamp;
        private float _verticalPressTimestamp;
 
        public override Vector2 ReadValue(ref InputBindingCompositeContext context) {
            var upPressed    = context.ReadValueAsButton(Up);
            var downPressed  = context.ReadValueAsButton(Down);
            var leftPressed  = context.ReadValueAsButton(Left);
            var rightPressed = context.ReadValueAsButton(Right);
 
            if (upPressed    && !_upPressedLastFrame)    _upPressTimestamp    = Time.time;
            if (downPressed  && !_downPressedLastFrame)  _downPressTimestamp  = Time.time;
            if (leftPressed  && !_leftPressedLastFrame)  _leftPressTimestamp  = Time.time;
            if (rightPressed && !_rightPressedLastFrame) _rightPressTimestamp = Time.time;

            if (upPressed || downPressed) _verticalPressTimestamp = Time.time;
            if (leftPressed || rightPressed) _horizontalPressTimestamp = Time.time;
        
            var x = (leftPressed, rightPressed) switch {
                (false, false)                                              =>  0f,
                (true,  false)                                              => -1f,
                (false, true)                                               =>  1f,
                (true,  true) when _rightPressTimestamp > _leftPressTimestamp =>  1f,
                (true,  true) when _rightPressTimestamp < _leftPressTimestamp => -1f,
                (true,  true)                                               =>  0f
            };
 
            var y = (downPressed, upPressed) switch {
                (false, false)                                           =>  0f,
                (true,  false)                                           => -1f,
                (false, true)                                            =>  1f,
                (true,  true) when _upPressTimestamp > _downPressTimestamp =>  1f,
                (true,  true) when _upPressTimestamp < _downPressTimestamp => -1f,
                (true,  true)                                            =>  0f
            };
 
            // Remove diagonal inputs
            if (x != 0f && y != 0f)
            {
                if (_horizontalPressTimestamp > _verticalPressTimestamp)
                {
                    y = 0f;
                }
                else
                {
                    x = 0f;
                }
            }
 
            _upPressedLastFrame    = upPressed;
            _downPressedLastFrame  = downPressed;
            _leftPressedLastFrame  = leftPressed;
            _rightPressedLastFrame = rightPressed;
 
            return new Vector2(x, y);
        }
 
        public override float EvaluateMagnitude(ref InputBindingCompositeContext context) {
            var value = ReadValue(ref context);
            return value.magnitude;
        }
 
#if UNITY_EDITOR
        static WASDComposite() {
            Initialize();
        }
#endif
 
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Initialize() {
            InputSystem.RegisterBindingComposite<WASDComposite>();
        }
    }
}