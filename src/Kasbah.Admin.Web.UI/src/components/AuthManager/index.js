import React from 'react'
import { connect } from 'react-redux'

export class AuthManager extends React.Component {
  static propTypes = {
    children: React.PropTypes.node
  }

  componentDidMount() {

  }

  render() {
    return this.props.children
  }
}

const mapDispatchToProps = {
}

const mapStateToProps = (state) => ({})

export default connect(mapStateToProps, mapDispatchToProps)(AuthManager)
