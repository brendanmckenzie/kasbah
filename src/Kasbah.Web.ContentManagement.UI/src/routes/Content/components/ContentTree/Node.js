import React from 'react'
import { Link } from 'react-router'
import ItemButton from 'components/ItemButton'
import NodeTree from './NodeTree'

class Node extends React.Component {
  static propTypes = {
    node: React.PropTypes.object.isRequired
  }

  static contextTypes = {
    onToggleNode: React.PropTypes.func.isRequired,
    expandedNodes: React.PropTypes.object.isRequired,
    types: React.PropTypes.array,
    readOnly: React.PropTypes.bool,
    onSelect: React.PropTypes.func
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

  get icon() {
    const { types } = this.context
    const type = types.filter(ent => ent.alias === this.props.node.type)[0]

    if (type) {
      if (type.icon) {
        const attrs = {
          className: `fa fa-${type.icon}`,
          style: {
            color: type.iconColour
          }
        }
        return <i {...attrs} />
      }
    }

    return <i className='fa fa-file-o' />
  }

  render() {
    const { node: { id, displayName, alias } } = this.props
    const { readOnly, onSelect } = this.context

    return (
      <li>
        <div className='level'>
          <div className='level-left'>
            <button
              className='button is-small expand level-item'
              onClick={this.handleToggleExpand}>
              <span className='icon is-small'>
                <i className={this.expanded ? 'fa fa-minus-square-o' : 'fa fa-plus-square-o'} />
              </span>
            </button>
            <span className='level-item icon is-small'>
              {this.icon}
            </span>
            {readOnly
              ? (<ItemButton
                item={this.props.node}
                className='button is-small is-link'
                onClick={onSelect}>{displayName || alias}</ItemButton>)
              : (<Link to={`/content/${id}`}
                className='level-item level-shrink'
                activeClassName='is-active'>{displayName || alias}</Link>)}
          </div>
        </div>
        {this.tree}
      </li>
    )
  }
}

export default Node
