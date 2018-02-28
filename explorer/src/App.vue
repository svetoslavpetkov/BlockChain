<template>
  <div id="vueJSApp">
      <div class="app-container container">
          <router-view></router-view>
      </div>
  </div>
</template>

<script>
export default {
  name: 'App',
  methods: {
    isActive (name) {
      return name === this.$route.name
    }
  }
}
</script>

<style>
#app {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}
</style>
