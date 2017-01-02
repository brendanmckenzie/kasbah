import React from 'react'
import Loading from 'components/Loading'
import NodeTree from './NodeTree'

class ContentTree extends React.Component {
  static propTypes = {
    describeTreeRequest: React.PropTypes.func.isRequired,
    describeTree: React.PropTypes.object.isRequired,
    createNodeRequest: React.PropTypes.func.isRequired
  }

  componentWillMount() {
    this.props.describeTreeRequest()
  }

  render() {
    if (this.props.describeTree.loading) {
      return <Loading />
    }

    return (
      <NodeTree tree={this.props.describeTree.payload} parent={null} createNodeRequest={this.props.createNodeRequest} />
    )
  }
}

export default ContentTree
