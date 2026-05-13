using System;
using System.Collections.Generic;

/// <summary>
/// 전역 이벤트 버스.
/// Control → UI 단방향 통신.
/// 
/// 사용법:
///   발행: EventBus.Emit(new OnWaveChanged { wave = 3 });
///   구독: EventBus.On<OnWaveChanged>(e => UpdateUI(e.wave));
///   해제: EventBus.Off<OnWaveChanged>(handler);
/// </summary>
public static class EventBus
{
  private static readonly Dictionary<Type, Delegate> _handlers = new ();
  //----------------------------------------------------------------------------
  /// <summary>이벤트 구독</summary>
  public static void On<T>(Action<T> handler)
  {
    var type = typeof(T);
    if (_handlers.ContainsKey(type))
      _handlers[type] = Delegate.Combine(_handlers[type], handler);
    else
      _handlers[type] = handler;
  }

  //----------------------------------------------------------------------------
  /// <summary>이벤트 구독 해제</summary>
  public static void Off<T>(Action<T> handler)
  {
    var type = typeof(T);
    if (_handlers.ContainsKey(type))    
      _handlers[type] = Delegate.Remove(_handlers[type], handler);               
  }

  //----------------------------------------------------------------------------
  /// <summary>이벤트 발행</summary>
  public static void Emit<T>(T eventData)
  {
    var type = typeof(T);
    if (_handlers.TryGetValue(type, out var handlers))
      (handlers as Action<T>)?.Invoke(eventData);    
  }

  //----------------------------------------------------------------------------
}
