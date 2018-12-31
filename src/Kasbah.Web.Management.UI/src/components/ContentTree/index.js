import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { actions as contentActions } from 'store/appReducers/content'
import Loading from 'components/Loading'
import NodeTree from './NodeTree'

class ContentTree extends React.Component {
  static propTypes = {
    describeTree: PropTypes.func.isRequired,
    listTypes: PropTypes.func.isRequired,
    content: PropTypes.object.isRequired,
    contextName: PropTypes.string.isRequired,
    readOnly: PropTypes.bool,
    selected: PropTypes.string,
    expandToNode: PropTypes.func.isRequired,
  }

  static childContextTypes = {
    contextName: PropTypes.string.isRequired,
    readOnly: PropTypes.bool,
  }

  getChildContext() {
    return {
      contextName: this.props.contextName,
      readOnly: this.props.readOnly,
    }
  }

  componentDidMount() {
    if (!this.props.content.tree.loaded) {
      this.props.describeTree()
    }
    if (!this.props.content.types.loaded) {
      this.props.listTypes()
    }
    if (this.props.selected) {
      this.props.expandToNode(this.props.context, this.props.selected)
    }
  }

  render() {
    if (this.props.content.tree.loading) {
      return <Loading />
    }

    return <NodeTree parent={null} />
  }
}

const mapStateToProps = (state) => ({
  content: state.content,
})

const mapDispatchToProps = {
  ...contentActions,
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ContentTree)
