import React from 'react'
import PropTypes from 'prop-types'

class ItemButton extends React.Component {
  handleClick = () => {
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
  item: PropTypes.any.isRequired,
  onClick: PropTypes.func.isRequired,
  children: PropTypes.any,
  className: PropTypes.string,
  type: PropTypes.string,
  action: PropTypes.any
}

export default ItemButton
