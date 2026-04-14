/// <summary>
/// Behaviour에서 결과를 알려준다.
/// </summary>
public enum Status
{
    Success, 
    Failure, 
    Running
};

/// <summary>
/// 모든 행동들의 최상위 부모이다.
/// BT에서 Behaviour은 Tree에서 Node를 의미한다.
/// Behaviour == Node라서 INode라고 적어도 되지만 여기서는 IBehaviour이라고 명시했다.
/// </summary>
public interface IBehaviour
{
    public Status Execute(Blackboard blackBoard);
}
