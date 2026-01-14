using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Rigidbody2D ��K���A�^�b�`������錾
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("�ʏ�E�Œᑬ�x�i������x���Ȃ�Ȃ��j")]
    public float baseSpeed = 2f;
    [Tooltip("�������̍ō����x")]
    public float maxSpeed = 6f;
    [Tooltip("D�L�[�ł̉����x�i�P�ʁF���x/�b�j")]
    public float acceleration = 4f;
    [Tooltip("A�L�[�ł̌����x�i�P�ʁF���x/�houhou b�j")]
    public float deceleration = 2f;

    [Header("Stamina / �X�^�~�i")]
    [Tooltip("PlayerStamina �X�N���v�g�� Inspector �Ŋ����Ă�")]
    public PlayerStamina stamina;                 // �� �ύX�����FPlayerStamina �ɓ���
    [Tooltip("�X�^�~�i����{��")]
    public float staminaDrainMultiplier = 1f;     // �� �ύX�����FPlayerStamina �ƘA�g

    [Header("Controls")]
    public KeyCode accelKey = KeyCode.D;
    public KeyCode decelKey = KeyCode.A;
    public KeyCode jamKey = KeyCode.J;            // �����W���}�L�[�p

    [Header("Behavior")]
    public bool applyVelocityInFixed = true;//�v�Z���e�̂Ԃ����ݕ��X�C�b�`

    private Rigidbody2D rb;
    private float nextFrameTargetSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        rb.velocity = new Vector2(baseSpeed, 0f);//new�̕K�v�����𒆁B
        nextFrameTargetSpeed = baseSpeed;
    }

    void Update()
    {
        // ===== �X�^�~�i���R�� =====
        if (stamina != null)
        {
            // D�L�[�������Ă��Ȃ���������
            if (!Input.GetKey(accelKey))
            {
                stamina.Regenerate(stamina.regeneRate * Time.deltaTime);//Regenerate�̃t���[���ˑ���Time.deltaTime�ŉ����B
                //stamina.regeneRate=�P�b������̉񕜗�

            }
        }
        //PlayerStamina.cs�ˑ��ӏ�--------------------------------------------------------------------------------


        //// ===== �v�Z���e�̂Ԃ����ݕ� =====
        float computedSpeed = ComputeSpeedFromInput();

        if (applyVelocityInFixed==true)
            nextFrameTargetSpeed = computedSpeed; //����Update�i�j�Ōv�Z���e���u�`���ށB
        else
            rb.velocity = new Vector2(computedSpeed, 0f); //����Fixed�i�j�Ɍv�Z���e���u�`���ށB 

        HandleJamInput();
        Debug.Log("Speed: " + rb.velocity.x);
        
    }

    void FixedUpdate()
    {
        if (applyVelocityInFixed)
        {
            rb.velocity = new Vector2(nextFrameTargetSpeed, 0f);
        }
    }

    /// <summary>
    /// ���͂Ɋ�Â����x�v�Z�{�X�^�~�i����
    /// </summary>
    private float ComputeSpeedFromInput()
    {
        float currentSpeed = rb.velocity.x;//���݂�Speed���擾�B

        // A�L�[�Ō����i�X�^�~�i����Ȃ��j���ݑ��x-�����l
        if (Input.GetKey(decelKey))
            currentSpeed -= deceleration * Time.deltaTime;//�����Ō���

        // D�L�[�ŉ����i�X�^�~�i���c���Ă���ꍇ�̂݁j���ݑ��x+�����l
        if (Input.GetKey(accelKey))
        {
            if (stamina == null)
            {
                // PlayerStamina ���ݒ莞�̓f�o�b�O�p�ɉ���
                currentSpeed += acceleration * Time.deltaTime;
            }
            else
            {
                if (!stamina.IsExhausted)//��J���Ă��Ȃ��ꍇ
                {
                    currentSpeed += acceleration * Time.deltaTime;//�����ŉ���
                  
                    // �X�^�~�i����iPlayerStamina ���� drainRate ���g�p�j �� �ύX����
                    float drain = stamina.drainRate * staminaDrainMultiplier * Time.deltaTime;
                    stamina.Drain(drain);
                }
                // Exhausted �Ȃ�����s��
            }
        }
        //�ʃX�N���v�g�ˑ��ӏ�--------------------------------------------------------------------------------

        // ���x�N�����v(���x����)���ݑ��xx = ���ݑ��x���Œᑬ�x�ƍō����x�̊Ԃ�Clamp�i���݂��ށj
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        return currentSpeed; //ComputeSpeedFromInput()�̍ŏ���CurrentSpeed�Ɍv�Z��̌��ݑ��x��n���B
    }

    /// <summary>
    /// �����́u���C�o�����W���}����v�A�N�V�����̎󂯌�
    /// </summary>
    private void HandleJamInput()
    {
        if (Input.GetKeyDown(jamKey))
        {
            Debug.Log("[Jam] �W���}�A�N�V���������i�������j");
        }
    }

    // �O�����猻�ݑ��x���擾�i�w���p�[�j
    public float GetCurrentSpeed()
    {
        return rb != null ? rb.velocity.x : 0f;
    }

    // �O������ڕW���x�������Z�b�g�i�f�o�b�O�p�j
    public void SetSpeed(float target)
    {
        float clamped = Mathf.Clamp(target, baseSpeed, maxSpeed);
        if (applyVelocityInFixed)
            nextFrameTargetSpeed = clamped;
        else if (rb != null)
            rb.velocity = new Vector2(clamped, 0f);
    }

    //��`:
    //    �v�Z���p��Update��Fixed�i�j�ւ̂Ԃ����ݕ��X�C�b�`(applyVelocityInFixe)
    //    ��
    // Start�i�j 
    //    nextframespeed��BaseSpeed
    //    ��
    //ComputeSpeedFromInput()
    //    ���������ꍇ�E���������ꍇ�����ꂼ��v�Z�B �Ƌ��ɉ��������ꍇ�̃X�^�~�i����ʂ�PlayerStaminacs�ƘA�g���Čv�Z
    //    ��
    // Update�i�j 
    //    �v�Z���ʂ�applyVelocityInFixed�ɂ���������nextframespeed�ɂԂ�����
    //    ��
    // Fixed�i�j
    //    nextframespeed�Ɋi�[���ꂽ�v�Z���ʂɏ]���� �����v�Z�B
}
