import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { push } from 'react-router-redux'

const withRouteOnEnter = callback => BaseComponent => {
  const routeOnEnterCallback = (props) => {
    if (callback && typeof callback === 'function') {
      callback(props)
    }
  }

  class RouteOnEnterComponent extends React.Component {
    static propTypes = {
      location: PropTypes.object.isRequired
    }

    componentWillMount() {
      routeOnEnterCallback(this.props)
    }

    componentWillReceiveProps(nextProps) {
      // not 100% sure about using `location.key` to distinguish between routes
      if (nextProps.location.key !== this.props.location.key) {
        routeOnEnterCallback(nextProps)
      }
    }

    render() {
      return <BaseComponent {...this.props} />
    }
  }

  const mapStateToProps = (state) => ({
    auth: state.auth
  })

  const mapDispatchToProps = {
    push
  }

  return connect(mapStateToProps, mapDispatchToProps)(RouteOnEnterComponent)
}

export default withRouteOnEnter
