using System.Collections.Generic;
/// <summary>
/// [Composite Node]
/// BT에서 Behaviour을 선택하는 노드다.
/// 일련의 단계를 순서대로 실행해서 Success or Running을 찾는 데 사용된다.
/// 왼쪽에서 오른쪽 순서로 실행하며 흐름을 제어한다.
/// 
/// **핵심 규칙**
/// 1. 하나의 자식이라도 Running or Running을 return 하면, 즉시 중단하고 해당 Status를 Return 한다.
/// 2. Failure를 반환하면 자식을 건너뛰고 다음 자식을 실행한다.
/// </summary>
public class Selector : BaseComposite
{
    public Selector(List<IBehaviour> children) : base(children) { }

    /// <summary>
    /// BaseComposite에 있는 Children을 돌면서 Success or Running 인지 확인한다.
    /// Running이나 Success가 있으면 즉시 해당 값을 return 한다.
    /// 이 함수를 실행하는 것만으로 BT 선택 순서 1번째 조건을 만족하는 것이다.
    /// </summary>
    /// <returns></returns>
    public override Status Execute(Blackboard blackBoard)
    {
        // BT 선택 순서에서 2번째 조건에 대한 내용이다. 
        // 왼쪽부터 오른쪽으로 검사한다.
        foreach(var child in children)
        {
            // BT 선택 순서에서 3번째 조건에 대한 내용이다.
            // Status를 확인해서 Success인지 Running이 되는지 안되는 지 확인한다.
            Status childStatus = child.Execute(blackBoard);

            switch(childStatus)
            {
                // Failure이라면 계속 실행한다.
                case Status.Failure:
                    continue;
                // 아래는 [중단 규칙]에 대한 내용이다. 
                case Status.Success:
                    return Status.Success;
                case Status.Running:
                    return Status.Running;
            }
        }

        // foreach를 다 돌았다면 전부 실패인 것이여서 Status를 Failure로 한다.
        return Status.Failure;
    }
}
