import React from 'react'
import { Link } from 'react-router'
import NodeTree from './NodeTree'

class Node extends React.Component {
  static propTypes = {
    node: React.PropTypes.object.isRequired,
    tree: React.PropTypes.array.isRequired,
    onCreateNode: React.PropTypes.func.isRequired
  }

  constructor() {
    super()

    this.state = { expanded: false }

    this.handleToggleExpand = this.handleToggleExpand.bind(this)
  }

  get tree() {
    if (!this.state.expanded) { return null }

    const { node, tree, onCreateNode } = this.props

    return <NodeTree parent={node.id} tree={tree} onCreateNode={onCreateNode} />
  }

  handleToggleExpand() {
    this.setState({
      expanded: !this.state.expanded
    })
  }

  render() {
    const { node } = this.props

    return (
      <li>
        <div className='level'>
          <div className='level-left'>
            <Link to={`/content/${node.id}`}
              className='level-item'
              activeClassName='is-active'>{node.displayName}</Link>
          </div>
          <div className='level-right'>
            <button
              className='button is-small add-node'
              onClick={this.handleToggleExpand}>
              <span className='icon is-small'>
                <i className={this.state.expanded ? 'fa fa-minus-square-o' : 'fa fa-plus-square-o'} />
              </span>
            </button>
          </div>
        </div>
        {this.tree}
      </li>
    )
  }
}

export default Node
