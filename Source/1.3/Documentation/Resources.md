# Resources

A resource is a collection of ThingDefs that gets produced by workers in a settlement each tax cycle

The amount of resources collected is determined by 1 \* TileModifiers \* \[Modifiers from the Settlement\]

## How to create a ResourceDef

### Create a new Defs file

- Go into your mods folder structure and create a "Defs" folder, if it doesn't exist yet
- Enter it and create an .xml file with any name you want
- Start it with this format:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
</Defs>
```

- Start working on a new ResourceDef by opening up a ResourceDef bracket thingy: (We recommend using our Empire_ResourceBase as parent, as it will fill in missing fields with base values)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```

- Fill in the `defName`, `Label`  and `description` fields. We recommend prefixing your `defName` with the name of your mod or a common prefix your mod uses. If needed or wanted, you can also define a `Empire_Rewritten.Resources.ResourceWorker` using the optional `resourceWorker` field. It can modify the internal `Verse.ThingFilter` using C#

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
		<defName>Empire_Example</defName>
		<label>Example</label>
		<description>This is a description resource. Don't let workers touch it</description>
	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```

- Give your resource an Icon by defining an `iconData` field containing a `texPath` field

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
		<defName>Empire_Example</defName>
		<label>Example</label>
		<description>This is a description resource. Don't let workers touch it</description>

		<iconData>
			<texPath>Resources/Example</texPath> <!-- This would point to YourMod/Textures/Resources/Example -->
		</iconData>
	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```

- Define the `ThingDefs` that can be gathered from the Resource using the `stuffCategories`, `thingCategoryDefs` and `allowedThingDefs` fields. You can remove `Thingdefs` using the `removeStuffCategoryDefs`, `removeThingCategoryDefs` and  `postRemoveThingDefs`  fields. (Order: `stuffCategoryDefs` => `removeStuffCategoryDefs` => `thingCategoryDefs` => `removeThingCategoryDefs` => `allowedThingDefs` => `postRemoveThingDefs` => `ResourceWorker`)

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
		<defName>Empire_Example</defName>
		<label>Example</label>
		<description>This is a description resource. Don't let workers touch it</description>

		<iconData>
			<texPath>Resources/Example</texPath> <!-- This would point to YourMod/Textures/Resources/Example -->
		</iconData>

		<!-- As an example, here is what mining uses -->
		<stuffCategoryDefs>
			<li>Metallic</li>
			<li>Stony</li>
		</stuffCategoryDefs>
		<thingCategoryDefs>
			<li>StoneBlocks</li>
		</thingCategoryDefs>
		<allowedThingDefs>
			<li>ComponentIndustrial</li>
		</allowedThingDefs>

		<!-- ThingDefs listed here will be unincluded after everything was added, so if you wanted to uninclude silver just put it's defName here-->
		<postRemoveThingDefs>
			<li>Silver</li>
		</postRemoveThingDefs>
	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```

- Next up are bonuses for the tile's "hilliness" and surrounding waterbodies. Here, factors are multipliers for the resource production and offsets are applied additively. Add them like so:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
		<defName>Empire_Example</defName>
		<label>Example</label>
		<description>This is a description resource. Don't let workers touch it</description>

		<iconData>
			<texPath>Resources/Example</texPath> <!-- This would point to YourMod/Textures/Resources/Example -->
		</iconData>

		<!-- As an example, here is what mining uses -->
		<stuffCategoryDefs>
			<li>Metallic</li>
			<li>Stony</li>
		</stuffCategoryDefs>
		<thingCategoryDefs>
			<li>StoneBlocks</li>
		</thingCategoryDefs>
		<allowedThingDefs>
			<li>ComponentIndustrial</li>
		</allowedThingDefs>

		<!-- ThingDefs listed here will be unincluded after everything was added, so if you wanted to uninclude silver just put it's defName here-->
		<postRemoveThingDefs>
			<li>Silver</li>
		</postRemoveThingDefs>

		<!-- These apply when the settlement this resource is produced on is on a tile with corresponding hilliness -->
		<hillinessFactors>
			<flat>1</flat>
			<smallHills>1</smallHills>
			<largeHills>1</largeHills>
			<mountainous>1</mountainous>
		</hillinessFactors>

		<hillinessOffsets>
			<flat>0</flat>
			<smallHills>0</smallHills>
			<largeHills>0</largeHills>
			<mountainous>0</mountainous>
		</hillinessOffsets>

		<!-- lake and ocean factors apply if a settlement borders a lake and/or ocean, river applies when a river flows through the settlement -->
		<waterBodyFactors>
			<lake>1</lake>
			<river>1</river>
			<ocean>1</ocean>
		</waterBodyFactors>

		<waterBodyOffsets>
			<lake>0</lake>
			<river>0</river>
			<ocean>0</ocean>
		</waterBodyOffsets>

	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```

- As a last step, define the curves, offsetting the resource gain based on tile properties:

```xml
<?xml version="1.0" encoding="utf-8" ?>
<Defs>
	<Empire_Rewritten.Resources.ResourceDef ParentName="Empire_ResourceBase">
		<defName>Empire_Example</defName>
		<label>Example</label>
		<description>This is a description resource. Don't let workers touch it</description>

		<iconData>
			<texPath>Resources/Example</texPath> <!-- This would point to YourMod/Textures/Resources/Example -->
		</iconData>

		<!-- As an example, here is what mining uses -->
		<stuffCategoryDefs>
			<li>Metallic</li>
			<li>Stony</li>
		</stuffCategoryDefs>
		<thingCategoryDefs>
			<li>StoneBlocks</li>
		</thingCategoryDefs>
		<allowedThingDefs>
			<li>ComponentIndustrial</li>
		</allowedThingDefs>

		<!-- ThingDefs listed here will be unincluded after everything was added, so if you wanted to uninclude silver just put it's defName here-->
		<postRemoveThingDefs>
			<li>Silver</li>
		</postRemoveThingDefs>

		<!-- These apply when the settlement this resource is produced on is on a tile with corresponding hilliness -->
		<hillinessFactors>
			<flat>1</flat>
			<smallHills>1</smallHills>
			<largeHills>1</largeHills>
			<mountainous>1</mountainous>
		</hillinessFactors>

		<hillinessOffsets>
			<flat>0</flat>
			<smallHills>0</smallHills>
			<largeHills>0</largeHills>
			<mountainous>0</mountainous>
		</hillinessOffsets>

		<!-- lake and ocean factors apply if a settlement borders a lake and/or ocean, river applies when a river flows through the settlement -->
		<waterBodyFactors>
			<lake>1</lake>
			<river>1</river>
			<ocean>1</ocean>
		</waterBodyFactors>

		<waterBodyOffsets>
			<lake>0</lake>
			<river>0</river>
			<ocean>0</ocean>
		</waterBodyOffsets>

		<!-- Important! The values provided in the Parent need to be overridden by including the Inherit="False" tag-->
		<!-- Anything below the first point will use that points value, anything higher than the last will use the last ones value -->
		<!-- All of these are added multiplikatively -->
		<!-- The values here are the ones provided in the parent class. If they are fitting for your resource, you can ignore the corresponding field -->
		<temperatureCurve Inherit="False">
			<points>
				<li>-270, 1.0</li>
				<li>   0, 1.0</li>
				<li>  50, 1.0</li>
			</points>
		</temperatureCurve>

		<rainfallCurve Inherit="False">
			<points>
				<li>   0,  1</li>
				<li>7500,  1</li>
			</points>
		</rainfallCurve>

		<heightCurve Inherit="False">
			<points>
				<li>   0, 1</li>
				<li>2500, 1</li>
			</points>
		</heightCurve>

		<swampinessCurve Inherit="False">
			<points>
				<li> 0.5, 1.0</li>
				<li> 0.6, 0.98</li>
				<li> 0.8, 0.9</li>
				<li> 1.0, 0.80</li>
			</points>
		</swampinessCurve>
	</Empire_Rewritten.Resources.ResourceDef>
</Defs>
```