import { defineAppSetup } from '@slidev/types'

export default defineAppSetup(({ router }) => {

  const getPresenterSlideId = (path: string) => {
    const match = path.match(/\/presenter\/([^/?#]+)/)
    return match?.[1]
  }
  const clickPresenterTimerPlay = () => {
    if (typeof document === 'undefined')
      return false

    const playIcon = document.querySelector<HTMLElement>('.slidev-presenter .grid-section.bottom .i-carbon\\:play')
    const toggleButton = playIcon?.parentElement as HTMLElement | null
    if (!toggleButton)
      return false

    toggleButton.click()
    return true
  }

  router.afterEach((to, from) => {

    const toSlide = getPresenterSlideId(to.path)
    const fromSlide = getPresenterSlideId(from.path)
    if (!toSlide || !fromSlide || toSlide === fromSlide)
      return
    requestAnimationFrame(() => {
      if (clickPresenterTimerPlay())
        return
      requestAnimationFrame(() => {
        clickPresenterTimerPlay()
      })
    })
  })
})
