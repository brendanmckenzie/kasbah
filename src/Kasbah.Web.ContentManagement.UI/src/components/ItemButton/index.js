import React from 'react'

class ItemButton extends React.Component {
  constructor () {
    super()

    this.handleClick = this.handleClick.bind(this)
  }

  handleClick () {
    this.props.onClick(this.props.item, this.props.action)
  }

  render () {
    const { children, className, type } = this.props
    return (
      <button className={className} onClick={this.handleClick} type={type}>
        {children}
      </button>
    )
  }
}

ItemButton.propTypes = {
  item: React.PropTypes.any.isRequired,
  onClick: React.PropTypes.func.isRequired,
  children: React.PropTypes.any,
  className: React.PropTypes.string,
  type: React.PropTypes.string,
  action: React.PropTypes.any
}

export default ItemButton
