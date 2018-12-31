import React from 'react'
import PropTypes from 'prop-types'

export class Tab extends React.Component {
  static propTypes = {
    children: PropTypes.any.isRequired,
  }

  render() {
    return this.props.children
  }
}

class TabHeader extends React.Component {
  static propTypes = {
    index: PropTypes.number.isRequired,
    currentIndex: PropTypes.number.isRequired,
    title: PropTypes.string.isRequired,
    onClick: PropTypes.func.isRequired,
  }

  handleClick = () => this.props.onClick(this.props.index)

  render() {
    const attrs = {
      className: this.props.index === this.props.currentIndex ? 'is-active' : null,
      onClick: this.handleClick,
    }
    return (
      <li {...attrs}>
        <button>{this.props.title}</button>
      </li>
    )
  }
}

export class Tabs extends React.Component {
  static propTypes = {
    children: PropTypes.arrayOf(PropTypes.node),
  }

  constructor() {
    super()

    this.state = { currentIndex: 0 }
  }

  setCurrentIndex = (currentIndex) => this.setState({ currentIndex })

  get tabs() {
    if (this.props.children instanceof Array) {
      if (this.props.children.length === 1) {
        return null
      } else {
        return (
          <div className="tabs">
            <ul>
              {this.props.children.map((ent, index) => {
                const attrs = {
                  key: index,
                  title: ent.props.title,
                  index: index,
                  currentIndex: this.state.currentIndex,
                  onClick: this.setCurrentIndex,
                }
                return <TabHeader {...attrs} />
              })}
            </ul>
          </div>
        )
      }
    }

    return null
  }

  render() {
    return (
      <div className="tabs__container">
        {this.tabs}
        {this.props.children[this.state.currentIndex]}
      </div>
    )
  }
}

export default Tabs
