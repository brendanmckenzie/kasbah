import React from 'react'
import { connect } from 'react-redux'
import { actions as contentActions } from 'store/appReducers/content'
import PropTypes from 'prop-types'
import NodeTree from './NodeTree'

class ContentTree extends React.Component {
  static propTypes = {
    describeTree: PropTypes.func.isRequired,
    content: PropTypes.object.isRequired
  }

  componentDidMount() {
    if (!this.props.content.tree.loaded) {
      this.props.describeTree()
    }
  }

  render() {
    return (
      <NodeTree parent={null} onCreateNode={this.handleCreateNode} />
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(ContentTree)
