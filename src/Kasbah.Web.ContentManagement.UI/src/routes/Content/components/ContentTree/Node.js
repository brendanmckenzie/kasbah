import React from 'react'
import { Link } from 'react-router'
import NodeTree from './NodeTree'

class Node extends React.Component {
  static propTypes = {
    node: React.PropTypes.object.isRequired
  }

  static contextTypes = {
    onToggleNode: React.PropTypes.func.isRequired,
    expandedNodes: React.PropTypes.object.isRequired
  }

  constructor() {
    super()

    this.handleToggleExpand = this.handleToggleExpand.bind(this)
  }

  get tree() {
    const { node: { id, alias } } = this.props

    if (!this.expanded) { return null }

    return <NodeTree parent={id} parentAlias={alias} />
  }

  handleToggleExpand() {
    const { node: { id } } = this.props

    this.context.onToggleNode(id)
  }

  get expanded() {
    const { node: { id } } = this.props

    return this.context.expandedNodes[id]
  }

  render() {
    const { node: { id, displayName } } = this.props

    return (
      <li>
        <div className='level'>
          <div className='level-left'>
            <Link to={`/content/${id}`}
              className='level-item level-shrink'
              activeClassName='is-active'>{displayName}</Link>
          </div>
          <div className='level-right'>
            <button
              className='button is-small expand'
              onClick={this.handleToggleExpand}>
              <span className='icon is-small'>
                <i className={this.expanded ? 'fa fa-minus-square-o' : 'fa fa-plus-square-o'} />
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
