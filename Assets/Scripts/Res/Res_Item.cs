//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: ResDefProto/Res_Item.proto
namespace Res_Table
{
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResItemInfo")]
  public partial class ResItemInfo : global::ProtoBuf.IExtensible
  {
    public ResItemInfo() {}
    
    private uint _id = default(uint);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name=@"id", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(uint))]
    public uint id
    {
      get { return _id; }
      set { _id = value; }
    }
    private byte[] _name = null;
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name=@"name", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] name
    {
      get { return _name; }
      set { _name = value; }
    }
    private byte[] _desc = null;
    [global::ProtoBuf.ProtoMember(3, IsRequired = false, Name=@"desc", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] desc
    {
      get { return _desc; }
      set { _desc = value; }
    }
    private int _type = default(int);
    [global::ProtoBuf.ProtoMember(4, IsRequired = false, Name=@"type", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int type
    {
      get { return _type; }
      set { _type = value; }
    }
    private int _stack_num = default(int);
    [global::ProtoBuf.ProtoMember(5, IsRequired = false, Name=@"stack_num", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int stack_num
    {
      get { return _stack_num; }
      set { _stack_num = value; }
    }
    private byte[] _icon = null;
    [global::ProtoBuf.ProtoMember(6, IsRequired = false, Name=@"icon", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue(null)]
    public byte[] icon
    {
      get { return _icon; }
      set { _icon = value; }
    }
    private int _slots = default(int);
    [global::ProtoBuf.ProtoMember(7, IsRequired = false, Name=@"slots", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int slots
    {
      get { return _slots; }
      set { _slots = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
  [global::System.Serializable, global::ProtoBuf.ProtoContract(Name=@"ResItemList")]
  public partial class ResItemList : global::ProtoBuf.IExtensible
  {
    public ResItemList() {}
    
    private readonly global::System.Collections.Generic.List<Res_Table.ResItemInfo> _list = new global::System.Collections.Generic.List<Res_Table.ResItemInfo>();
    [global::ProtoBuf.ProtoMember(1, Name=@"list", DataFormat = global::ProtoBuf.DataFormat.Default)]
    public global::System.Collections.Generic.List<Res_Table.ResItemInfo> list
    {
      get { return _list; }
    }
  
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
      { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
  }
  
    [global::ProtoBuf.ProtoContract(Name=@"EquipSlot")]
    public enum EquipSlot
    {
            
      [global::ProtoBuf.ProtoEnum(Name=@"E_HEAD", Value=1)]
      E_HEAD = 1
    }
  
}