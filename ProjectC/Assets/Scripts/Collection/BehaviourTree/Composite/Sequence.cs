using System.Collections.Generic;
/// <summary>
/// [Composite Node]
/// BT를 순차적으로 실행하는 노드다.
/// 일련의 단계를 순서대로 실행하는 데 사용된다.
/// 왼쪽에서 오른쪽 순서로 실행하며 흐름을 제어한다.
/// 
/// **핵심 규칙**
/// 1. 모든 자식 노드가 Success를 반환해야 최종적으로 Success를 반환한다.
/// 2. 하나의 자식이라도 Failure or Running을 return 하면, 즉시 중단하고 해당 Status를 Return 한다.
/// </summary>
public class Sequence : BaseComposite
{
    public Sequence(List<IBehaviour> children) : base(children) { }

    /// <summary>
    /// BaseComposite에 있는 Children을 돌면서 Success인지 확인한다.
    /// Running이나 Failure가 있으면 즉시 해당 값을 return 한다.
    /// 이 함수를 실행하는 것만으로 BT 실행 순서 1번째 조건을 만족하는 것이다.
    /// </summary>
    /// <returns></returns>
    public override Status Execute(Blackboard blackBoard)
    {
        // BT 실행 순서에서 2번째 조건에 대한 내용이다. 
        // 왼쪽부터 오른쪽으로 검사한다.

        foreach(var child in children)
        {
            // BT 실행 순서에서 3번째 조건에 대한 내용이다.
            // Status를 확인해서 Success가 되는지 안되는 지 확인한다.
            Status childStatus = child.Execute(blackBoard);

            switch(childStatus)
            {
                // Success라면 계속 실행한다.
                case Status.Success:
                    continue;
                // 아래는 [중단 규칙]에 대한 내용이다. 
                case Status.Failure:
                    return Status.Failure;
                case Status.Running:
                    return Status.Running;
            }
        }

        // foreach를 다 돌았다면 전부 성공인 것이여서 Status를 Success로 한다.
        return Status.Success;
    }

}
