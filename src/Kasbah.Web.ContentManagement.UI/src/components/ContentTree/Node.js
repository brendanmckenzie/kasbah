import React from 'react'
import PropTypes from 'prop-types'
import { connect } from 'react-redux'
import { NavLink } from 'react-router-dom'
import { actions as contentActions } from 'store/appReducers/content'
import ItemButton from 'components/ItemButton'
import NodeTree from './NodeTree'
import NodeIcon from './NodeIcon'

class Node extends React.Component {
  static propTypes = {
    node: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    toggleNode: PropTypes.func.isRequired,
    selectNode: PropTypes.func.isRequired
  }

  static contextTypes = {
    contextName: PropTypes.string.isRequired,
    readOnly: PropTypes.bool
  }

  handleToggleExpand = () => {
    this.props.toggleNode(this.context.contextName, this.props.node.id)
  }

  get expanded() {
    const nodeState = this.props.content.nodeState

    return nodeState[this.context.contextName] && nodeState[this.context.contextName][this.props.node.id]
  }

  get tree() {
    const { node: { id, alias } } = this.props

    if (!this.expanded) { return null }

    return <NodeTree parent={id} parentAlias={alias} />
  }

  render() {
    const { node: { id, displayName, alias }, selectNode } = this.props
    const { readOnly } = this.context

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
              <NodeIcon {...this.props} />
            </span>
            {readOnly
              ? (<ItemButton
                item={this.props.node}
                className='button is-small is-link'
                onClick={selectNode}>{displayName || alias}</ItemButton>)
              : (<NavLink to={`/content/${id}`}
                className='level-item level-shrink'
                activeClassName='is-active'>{displayName || alias}</NavLink>)}
          </div>
        </div>
        {this.tree}
      </li>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content
})

const mapDispatchToProps = {
  ...contentActions
}

export default connect(mapStateToProps, mapDispatchToProps)(Node)
