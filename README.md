# MB2MultiCheats

## 摘要

本MOD全方位提升玩家及家族成员能力，涉及新增兵种、竞技大会、技能升级、角色升级、锻造、定居点、生育七个方面。欢迎大家试用。


## 前言

本人不善于英文表述，Mod内英文翻译为机翻，不足之处，请见谅。

本模组是个人在游玩过程中，根据自己需求，并参考借鉴大量其它优秀模组，反编译研究融合而成，仅部分代码为原创。
模组依赖四前置，如果游戏本体后续升级存在兼容性问题，只要基于最新版本重新编译应该即可解决。

[本模组][2]在GitHub完全开源（[MB2MultiCheats][1]），遵循MIT协议。实现方式力求简洁清楚，以便有部分编码经验的朋友二次开发和个性化修改。

## 功能说明

- 新增兵种
    - 新增兵种1个
    - 新增装备5个（护腕、弓、箭、马、马甲）
- 竞技大会
    - 玩家获胜奖励自定义兵种，可调节随机奖励数量
    - 玩家获胜奖励自定义装备，可调节装备奖励概率
- 技能升级
    - 六维最大属性点
    - 技能最大专精点
    - 学习效率加成，技能界面仍显示加成前的倍率，实际生效
    - 技能升级时，自动同时激活Perk
- 角色升级
    - 升级额外奖励属性点
    - 升级额外奖励专精点
    - 同伴对话后追加等级额外点数
- 锻造
    - 锻造不消耗体力
    - 锻造界面显示物品价值
    - 配件解锁速度加成
    - 自由锻造经验加成，方便角色快速升级
- 定居点
    - 每日奖励忠诚
- 选项
    - 禁用孕妇难产死亡
    - 禁用婴儿小产死亡
    - 战利品价值关联流氓习气等级，并均分优质前缀概率


## 附录一、简易开发说明

- 安装`VS2022`、目标框架：`.NET Framework 4.7.2`
- 导入项目后配置引用三个路径后即可编译(调试 -> MultiCheats属性)
    - `<MB2_INSTALL_DIR>\bin\Win64_Shipping_Client\`
    - `<MB2_INSTALL_DIR>\Modules\Bannerlord.Harmony\bin\Win64_Shipping_Client\`
    - `<MB2_INSTALL_DIR>\Modules\Bannerlord.MBOptionScreen\bin\Win64_Shipping_Client\`

## 附录二、属性专精参考表

```shell
技能学习上限(软) = 专精*30 + (属性-1)*10
技能最大上限(硬) = 专精*40 + (属性-1)*14 + 4
每级属性 = 属性*6 / 角色等级
每级专精 = 专精*6*3 / 角色等级
```

| 角色等级 | 属性    | 专精    | 技能学习上限 | 技能最大上限 | 每级属性 | 每级专精 |
| -------- | ------- | ------- | ------------ | ------------ | -------- | -------- |
| 60       | 10      | 5       | 240          | 330          | 1        | 1.5      |
| 60       | 30      | 15      | 740          | 1010         | 3        | 4.5      |
| 60       | 59(max) | 5       | 730          | 1016         | 5.9      | 1.5      |
| 60       | 10      | 22(max) | 750          | 1010         | 1        | 6.6      |
| 25       | 20      | 10      | 490(玩家)    | 670          | 4.8      | 7.2      |
| 15       | 10      | 5       | 240(NPC)     | 330          | 4        | 6        |


## 致谢

- [重要参考 已采纳 SmithForever @CalSev](https://www.nexusmods.com/mountandblade2bannerlord/mods/41)
- [重要参考 已采纳 SmithValue @BMan](https://www.nexusmods.com/mountandblade2bannerlord/mods/3184)
- [重要参考 已采纳 HighLoyalty @Jim136](https://www.nexusmods.com/mountandblade2bannerlord/mods/4933)
- [重要参考 已采纳 AutoTakePerks @Fengzi233](https://www.nexusmods.com/mountandblade2bannerlord/mods/4186)
- [重要参考 已采纳 OneAttributeAndTwoFocusPerLevel @newpaladinx333](https://www.nexusmods.com/mountandblade2bannerlord/mods/2165)
- [重要参考 已开源 Kaoses tweaks @Kaoses](https://www.nexusmods.com/mountandblade2bannerlord/mods/2911)

[1]: https://github.com/gnzhoutian/MB2MultiCheats.git  "MB2MultiCheats"
[2]: https://www.nexusmods.com/mountandblade2bannerlord/mods/7470  "MultiCheats"
