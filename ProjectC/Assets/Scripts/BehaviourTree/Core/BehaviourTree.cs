using UnityEngine;

public class BehaviourTree : MonoBehaviour
{
    private IBehaviour rootNode;
    private Blackboard blackBoard;

    public void SetUp(IBehaviour root, Blackboard _blackBoard)
    {
        if(root != null)
            this.rootNode = root;
        
        if(_blackBoard != null)
            this.blackBoard = _blackBoard;
    }

    public Status Tick()
    {
        if (rootNode == null) {
            return Status.Failure;
        }

        return rootNode.Execute(blackBoard);
    }

    void Start()
    {
        if (blackBoard == null)
            blackBoard = new Blackboard();
    }

    void Update()
    {
        Tick();
    }
}
