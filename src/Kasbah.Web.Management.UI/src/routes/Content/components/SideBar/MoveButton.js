import React from 'react'
import { connect } from 'react-redux'
import PropTypes from 'prop-types'
import { actions as uiActions } from 'store/appReducers/ui'
import { actions as contentActions } from 'store/appReducers/content'
import ContentTree from 'components/ContentTree'

class MoveButton extends React.Component {
  static propTypes = {
    node: PropTypes.object.isRequired,
    content: PropTypes.object.isRequired,
    showModal: PropTypes.func.isRequired,
    hideModal: PropTypes.func.isRequired,
    moveNode: PropTypes.func.isRequired,
  }

  handleClick = () => {
    const contextName = 'moveNode'
    const { node } = this.props

    const doMove = () => {
      const target = this.props.content.selection[contextName]
      if (target && target !== node.parent) {
        this.props.moveNode(node.id, target)
      }
    }
    const title = 'Move node'
    const control = <ContentTree contextName={contextName} selected={node.parent} readOnly />
    const buttons = [
      <button key="cancel" type="button" className="button" onClick={this.props.hideModal}>
        Cancel
      </button>,
      <button key="move" type="button" className="button is-primary" onClick={doMove}>
        Move
      </button>,
    ]

    this.props.showModal(title, control, buttons)
  }

  render() {
    return (
      <button className="button" type="button" onClick={this.handleClick}>
        <span className="icon is-small">
          <i className="fa fa-long-arrow-right" />
        </span>
        <span>Move</span>
      </button>
    )
  }
}

const mapStateToProps = (state, ownProps) => ({
  content: state.content,
})

const mapDispatchToProps = {
  ...uiActions,
  ...contentActions,
}

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(MoveButton)
