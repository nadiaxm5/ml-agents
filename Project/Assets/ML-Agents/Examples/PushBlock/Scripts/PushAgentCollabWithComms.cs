//Put this script on your blue cube.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PushAgentCollabWithComms : Agent
{

    private PushBlockSettings m_PushBlockSettings;
    private Rigidbody m_AgentRb;  //cached on initialization
    private PushBlockEnvControllerWithComms envController;
    public float[] previousMessage = new float[4];
    public float[] m0 = new float[4];
    public float[] m1 = new float[4];
    public float[] m2 = new float[4];
    public float[] m3 = new float[4];

    protected override void Awake()
    {
        base.Awake();
        envController = GetComponentInParent<PushBlockEnvControllerWithComms>();
        m_PushBlockSettings = FindObjectOfType<PushBlockSettings>();
    }

    public override void Initialize()
    {
        // Cache the agent rb
        m_AgentRb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Moves the agent according to the selected action.
    /// </summary>
    // public void MoveAgent(ActionSegment<int> act)
    public void MoveAgent(ActionBuffers act)
    {
        var continuousActions = act.ContinuousActions;
        var discreteActions = act.DiscreteActions;


        //ADD FORCE
        var inputV = continuousActions[0];
        var inputH = continuousActions[1];
        var rotate = continuousActions[2];

        var moveDir = transform.TransformDirection(new Vector3(inputH, 0, inputV));
        Quaternion rotAmount = Quaternion.AngleAxis(Time.fixedDeltaTime * 200f * rotate, Vector3.up);
        m_AgentRb.MoveRotation(m_AgentRb.rotation * rotAmount);
        m_AgentRb.AddForce(moveDir * m_PushBlockSettings.agentRunSpeed,
            ForceMode.VelocityChange);
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        foreach (var item in envController.AgentsList)
        {
            if (item.Agent != this)
            {
                // sensor.AddObservation(item.Agent.previousMessage);
                sensor.AddObservation(item.Agent.m0);
                sensor.AddObservation(item.Agent.m1);
                sensor.AddObservation(item.Agent.m2);
                sensor.AddObservation(item.Agent.m3);
            }
        }
    }

    // [SerializeField]
    // public float[,,,] word = new float[0,0,0,0];
    // public float[,,,] sentence = new float[0,0,0,0];

    // public List<float, float> messageList = new List<float, float>;
    // public [][] x =
    /// <summary>
    /// Called every step of the engine. Here the agent takes an action.
    /// </summary>
    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        // Move the agent using the action.
        MoveAgent(actionBuffers);

        var branch0 = actionBuffers.DiscreteActions[0];
        var branch1 = actionBuffers.DiscreteActions[1];
        var branch2 = actionBuffers.DiscreteActions[2];
        var branch3 = actionBuffers.DiscreteActions[3];
        previousMessage = new float[] { branch0, branch1, branch2, branch3 };



        var b0 = new float[] { 0, 0, 0, 0 };
        var b1 = new float[] { 0, 0, 0, 0 };
        var b2 = new float[] { 0, 0, 0, 0 };
        var b3 = new float[] { 0, 0, 0, 0 };
        b0[branch0] = 1;
        b1[branch1] = 1;
        b2[branch2] = 1;
        b3[branch3] = 1;
        m0 = b0;
        m1 = b1;
        m2 = b2;
        m3 = b3;




        /*  sentence [ [0,0,1,0], [1,0,1,0] ]
                word [0,1,0,0]
                    letters 0 || 1
        */

        //branch0
        // actionBuffers.DiscreteActions[0]
        // messageArray[0,1,0,0] = 4;
        // BitArray bitArray = new BitArray(4);
        // for (float i = 0; i < 2; i++)
        // {
        //     for (float j = 0; j < 2; j++)
        //     {
        //         for (float k = 0; k < 2; k++)
        //         {
        //             for (float l = 0; l < 2; l++)
        //             {
        //                var x = new float[,,,]{i,j,k,l};
        //             }
        //         }
        //     }
        // }

        // //option1
        // float[] message0 = new float[] {0,0,0,0};
        // float[] message1 = new float[] {1,0,0,0};
        //
        // discreteAction[0] = message0;
        // discreteAction[1] = message1;
        // //option2
        // var m0p0, m0p1, m0p2, m0p3 = 0
        // float[] message0 = [m0p0,m0p1,m0p2,m0p3];

    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var contActs = actionsOut.ContinuousActions;
        contActs[0] = Input.GetAxisRaw("Vertical");
        contActs[1] = Input.GetAxisRaw("Horizontal");
        var rotLeft = Input.GetKey(KeyCode.J);
        var rotRight = Input.GetKey(KeyCode.K);
        var rotDir = rotLeft && rotRight ? 0 : rotLeft && !rotRight ? -1 : !rotLeft && rotRight ? 1 : 0;
        contActs[2] = rotDir;
    }
}
