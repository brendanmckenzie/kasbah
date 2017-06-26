import React from 'react'
import PropTypes from 'prop-types'
import { Link } from 'react-router'
import ItemButton from 'components/ItemButton'
import NodeTree from './NodeTree'

class Node extends React.Component {
  static propTypes = {
    node: PropTypes.object.isRequired
  }

  static contextTypes = {
    onToggleNode: PropTypes.func.isRequired,
    expandedNodes: PropTypes.object.isRequired,
    types: PropTypes.array,
    readOnly: PropTypes.bool,
    onSelect: PropTypes.func
  }

  get tree() {
    const { node: { id, alias } } = this.props

    if (!this.expanded) { return null }

    return <NodeTree parent={id} parentAlias={alias} />
  }

  handleToggleExpand = () => {
    const { node: { id } } = this.props

    this.context.onToggleNode(id)
  }

  get expanded() {
    const { node: { id } } = this.props

    return this.context.expandedNodes[id]
  }

  get icon() {
    const { types } = this.context
    const type = types.find(ent => ent.alias.split(',')[0] === this.props.node.type.split(',')[0])

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

    return <i className='fa fa-file' />
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
                <i className={this.expanded ? 'fa fa-minus-circle' : 'fa fa-plus-circle'} />
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
