<template>
	<div id="app">
		<router-view />
	</div>
</template>

<style lang="less">
	html,
	body {
		margin: 0;
		padding: 0;
		height: 100%;
		overflow: hidden;
		font-family: "Helvetica Neue", Helvetica, "PingFang SC", "Hiragino Sans GB",
			"Microsoft YaHei", "微软雅黑", Arial, sans-serif;
	}

	#app {
		-webkit-font-smoothing: antialiased;
		-moz-osx-font-smoothing: grayscale;
		height: 100%;
		overflow: hidden;
	}
</style>
